using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class EdgeCompiler
{
    static readonly Regex referenceRegex = new Regex(@"^[\ \t]*(?:\/{2})?\#r[\ \t]+""([^""]+)""", RegexOptions.Multiline);
    static readonly Regex usingRegex = new Regex(@"^[\ \t]*(using[\ \t]+[^\ \t]+[\ \t]*\;)", RegexOptions.Multiline);
    static readonly bool debuggingEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EDGE_CS_DEBUG"));
    static readonly bool debuggingSelfEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EDGE_CS_DEBUG_SELF"));
    static Dictionary<string, Dictionary<string, Assembly>> referencedAssemblies = new Dictionary<string, Dictionary<string, Assembly>>();

    static EdgeCompiler()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        Assembly result = null;
        Dictionary<string, Assembly> requesting;
        if (referencedAssemblies.TryGetValue(args.RequestingAssembly.FullName, out requesting))
        {
            requesting.TryGetValue(args.Name, out result);
        }

        return result;
    }

    public Func<object, Task<object>> CompileFunc(IDictionary<string, object> parameters)
    {
        string source = (string)parameters["source"];
        string lineDirective = string.Empty;
        string fileName = null;
        int lineNumber = 1;

        // read source from file
        if (source.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase)
            || source.EndsWith(".csx", StringComparison.InvariantCultureIgnoreCase))
        {
            // retain fileName for debugging purposes
            if (debuggingEnabled)
            {
                fileName = source;
            }

            source = File.ReadAllText(source);
        }

        // add assembly references provided explicitly through parameters
        List<string> references = new List<string>();
        object v;
        if (parameters.TryGetValue("references", out v))
        {
            foreach (object reference in (object[])v)
            {
                references.Add((string)reference);
            }
        }

        // add assembly references provided in code as [//]#r "assemblyname" lines
        Match match = referenceRegex.Match(source);
        while (match.Success)
        {
            references.Add(match.Groups[1].Value);
            source = source.Substring(0, match.Index) + source.Substring(match.Index + match.Length);
            match = referenceRegex.Match(source);
        }

        if (debuggingEnabled)
        {
            object jsFileName;
            if (parameters.TryGetValue("jsFileName", out jsFileName))
            {
                fileName = (string)jsFileName;
                lineNumber = (int)parameters["jsLineNumber"];
            }
            
            if (!string.IsNullOrEmpty(fileName)) 
            {
                lineDirective = string.Format("#line {0} \"{1}\"\n", lineNumber, fileName);
            }
        }

        // try to compile source code as a class library
        Assembly assembly;
        string errorsClass;
        if (!this.TryCompile(lineDirective + source, references, out errorsClass, out assembly))
        {
            // try to compile source code as an async lambda expression

            // extract using statements first
            string usings = "";
            match = usingRegex.Match(source);
            while (match.Success)
            {
                usings += match.Groups[1].Value;
                source = source.Substring(0, match.Index) + source.Substring(match.Index + match.Length);
                match = usingRegex.Match(source);
            }

            string errorsLambda;
            source = 
                usings + "using System;\n"
                + "using System.Threading.Tasks;\n"
                + "public class Startup {\n"
                + "    public async Task<object> Invoke(object ___input) {\n"
                + lineDirective
                + "        Func<object, Task<object>> func = " + source + ";\n"
                + "#line hidden\n"
                + "        return await func(___input);\n"
                + "    }\n"
                + "}";

            if (debuggingSelfEnabled)
            {
                Console.WriteLine("Edge-cs trying to compile async lambda expression:");
                Console.WriteLine(source);
            }

            if (!TryCompile(source, references, out errorsLambda, out assembly))
            {
                throw new InvalidOperationException(
                    "Unable to compile C# code.\n----> Errors when compiling as a CLR library:\n"
                    + errorsClass
                    + "\n----> Errors when compiling as a CLR async lambda expression:\n"
                    + errorsLambda);
            }
        }

        // store referenced assemblies to help resolve them at runtime from AppDomain.AssemblyResolve
        referencedAssemblies[assembly.FullName] = new Dictionary<string, Assembly>();
        foreach (var reference in references)
        {
            try
            {
                var referencedAssembly = Assembly.UnsafeLoadFrom(reference);
                referencedAssemblies[assembly.FullName][referencedAssembly.FullName] = referencedAssembly;
            }
            catch
            {
                // empty - best effort
            }
        }

        // extract the entry point to a class method
        Type startupType = assembly.GetType((string)parameters["typeName"], true, true);
        object instance = Activator.CreateInstance(startupType, false);
        MethodInfo invokeMethod = startupType.GetMethod((string)parameters["methodName"], BindingFlags.Instance | BindingFlags.Public);
        if (invokeMethod == null)
        {
            throw new InvalidOperationException("Unable to access CLR method to wrap through reflection. Make sure it is a public instance method.");
        }

        // create a Func<object,Task<object>> delegate around the method invocation using reflection
        Func<object,Task<object>> result = (input) => 
        {
            return (Task<object>)invokeMethod.Invoke(instance, new object[] { input });
        };

        return result;
    }

    bool TryCompile(string source, List<string> references, out string errors, out Assembly assembly)
    {
        bool result = false;
        assembly = null;
        errors = null;

        Dictionary<string, string> options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
        CSharpCodeProvider csc = new CSharpCodeProvider(options);
        CompilerParameters parameters = new CompilerParameters();
        parameters.GenerateInMemory = true;
        parameters.IncludeDebugInformation = debuggingEnabled;
        parameters.ReferencedAssemblies.AddRange(references.ToArray());
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add("System.Core.dll");
        parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
        CompilerResults results = csc.CompileAssemblyFromSource(parameters, source);
        if (results.Errors.HasErrors)
        {
            foreach (CompilerError error in results.Errors)
            {
                if (errors == null)
                {
                    errors = error.ToString();
                }
                else
                {
                    errors += "\n" + error.ToString();
                }
            }
        }
        else
        {
            assembly = results.CompiledAssembly;
            result = true;
        }

        return result;
    }
}

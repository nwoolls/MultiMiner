using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.UX.IO
{
    public class CommandProcessor
    {
        public class Command
        {
            public string Name;
            public string Alias;
            public string Syntax;
            public Func<string[], bool> Handler;
        }

        private readonly List<Command> commands = new List<Command>();

        private readonly Action<string> syntaxCallback;
        private readonly Action<string> helpCallback;
        public CommandProcessor(Action<string> syntaxCallback, Action<string> helpCallback)
        {
            this.syntaxCallback = syntaxCallback;
            this.helpCallback = helpCallback;
        }

        public void RegisterCommand(string name, string alias, string syntax, Func<string[], bool> handler)
        {
            var command = new Command
            {
                Name = name.ToLower(),
                Alias = alias.ToLower(),
                Syntax = syntax,
                Handler = handler
            };

            commands.Add(command);
        }

        public bool ProcessCommand(string input)
        {
            var words = input.Split(' ');
            var firstWord = words.First().TrimStart('/').ToLower();
            var command = commands.SingleOrDefault(c => c.Name.Equals(firstWord) || c.Alias.Equals(firstWord));
            if (command == null) return false;
            if (!command.Handler(words)) syntaxCallback(command.Syntax);
            return true;
        }

        public void OutputHelp()
        {
            commands
                .OrderBy(c => c.Name)
                .ToList()
                .ForEach(c => helpCallback(String.Format("{0}{1} {2}", c.Name, String.IsNullOrEmpty(c.Alias) ? String.Empty : "|" + c.Alias, c.Syntax)));
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;

namespace MultiMiner.Utility.Parsers
{
    public class IniFileParser
    {
        private readonly Dictionary<string, string> keyPairs = new Dictionary<string, string>();
        
        public IniFileParser(string iniPath)
        {
            LoadIniFile(iniPath);
        }

        private void LoadIniFile(string iniPath)
        {
            if (File.Exists(iniPath))
            {
                TextReader iniFile = new StreamReader(iniPath);
                try
                {
                    string currentRoot = null;
                    string strLine = iniFile.ReadLine();

                    while (strLine != null)
                    {
                        strLine = strLine.TrimStart();

                        if (!String.IsNullOrEmpty(strLine) && !strLine.StartsWith("#") && !strLine.StartsWith(";"))
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            else
                            {
                                string[] keyPair = strLine.Split(new char[] { '=' }, 2);
                                string value = keyPair.Length > 1 ? keyPair[1].TrimStart() : null;

                                if (currentRoot == null)
                                    currentRoot = "ROOT";                                

                                string key = GetKey(currentRoot, keyPair[0]);
                                keyPairs[key] = value;
                            }
                        }
                        strLine = iniFile.ReadLine();
                    }
                }
                finally
                {
                    iniFile.Close();
                }
            }
            else
                throw new FileNotFoundException("Unable to locate " + iniPath);
        }

        private static string GetKey(string sectionName, string settingName)
        {
            return String.Format("{0}-{1}", sectionName.TrimEnd().ToUpper(), settingName.TrimEnd().ToUpper());
        }

        public string GetValue(string sectionName, string settingName)
        {
            string key = GetKey(sectionName, settingName);
            return keyPairs[key];
        }
    }
}
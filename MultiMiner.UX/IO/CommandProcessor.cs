using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.UX.IO
{
    public class CommandProcessor
    {
        private readonly Dictionary<string, Action<string[]>> commandHandlers = new Dictionary<string, Action<string[]>>();

        public void RegisterCommand(string name, string alias, Action<string[]> handler)
        {
            commandHandlers[name.ToLower()] = handler;
            if (!String.IsNullOrEmpty(alias))
                commandHandlers[alias.ToLower()] = handler;
        }

        public bool ProcessCommand(string input)
        {
            var words = input.Split(' ');
            var firstWord = words.First().TrimStart('/');
            if (!commandHandlers.ContainsKey(firstWord.ToLower())) return false;
            commandHandlers[firstWord.ToLower()](words);
            return true;
        }
    }
}

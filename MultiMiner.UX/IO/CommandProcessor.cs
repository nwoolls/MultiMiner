using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.UX.IO
{
    public class CommandProcessor
    {
        private readonly Dictionary<string, Action<string>> commandHandlers = new Dictionary<string, Action<string>>();

        public void RegisterCommand(string name, string alias, Action<string> handler)
        {
            commandHandlers[name.ToLower()] = handler;
            if (!String.IsNullOrEmpty(alias))
                commandHandlers[alias.ToLower()] = handler;
        }

        public bool ProcessCommand(string input)
        {
            var commandName = input.Split(' ').First().TrimStart('/');
            if (!commandHandlers.ContainsKey(commandName.ToLower())) return false;
            commandHandlers[commandName.ToLower()](input);
            return true;
        }
    }
}

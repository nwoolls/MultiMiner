using System;

namespace MultiMiner.MobileMiner.Data
{
    public class RemoteCommand
    {
        public class CommandMachine
        {
            public string Name { get; set; }
        }

        public int Id { get; set; }
        public string CommandText { get; set; }
        public DateTime CommandDate { get; set; }
        public CommandMachine Machine { get; set; }
    }
}

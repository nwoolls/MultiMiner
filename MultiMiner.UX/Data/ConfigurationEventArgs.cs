using System;

namespace MultiMiner.UX.Data
{
    public class ConfigurationEventArgs : EventArgs
    {
        public Configuration.Application Application;
        public Engine.Data.Configuration.Engine Engine;
        public Configuration.Perks Perks;
        public Configuration.Paths Paths;
        public bool ConfigurationModified;
    }
}

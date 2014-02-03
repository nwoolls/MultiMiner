using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using System.IO;

namespace MultiMiner.Win.Configuration
{
    public class PathConfiguration
    {
        public string SharedConfigPath { get; set; }

        public static string PathConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "PathConfiguration.xml");
        }

        public void SavePathConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, PathConfigurationFileName());
        }

        public void LoadPathConfiguration()
        {
            PathConfiguration tmp = ConfigurationReaderWriter.ReadConfiguration<PathConfiguration>(PathConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }
    }
}

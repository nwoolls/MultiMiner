using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.UX.Data.Configuration
{
    [XmlType(TypeName = "PathConfiguration")]
    public class Paths
    {
        public string SharedConfigPath { get; set; }

        private static string PathConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "PathConfiguration.xml");
        }

        public void SavePathConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, PathConfigurationFileName());
        }

        public void LoadPathConfiguration()
        {
            Paths tmp = ConfigurationReaderWriter.ReadConfiguration<Paths>(PathConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }
    }
}

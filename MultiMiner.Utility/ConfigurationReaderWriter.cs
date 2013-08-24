using System;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.Utility
{
    public static class ConfigurationReaderWriter
    {
        public static T ReadConfiguration<T>(string fileName) where T : new()
        {
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (TextReader reader = new StreamReader(fileName))
                    return (T)serializer.Deserialize(reader);
            }

            return new T();
        }

        public static void WriteConfiguration(object source, string fileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            Type type = source.GetType();
            XmlSerializer serializer = new XmlSerializer(type);
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, source);
            }
        }
    }
}

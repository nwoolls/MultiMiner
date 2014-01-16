using System;
using System.IO;
using System.Windows.Forms;
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
                {
                    T result;
                    try
                    {
                        result = (T)serializer.Deserialize(reader);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(String.Format("The file {0} was 0 bytes (likely due to a previous crash).\n\nDefault settings for this file have been loaded.", 
                            Path.GetFileName(fileName)), "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = new T();
                    }
                    return result;
                }
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

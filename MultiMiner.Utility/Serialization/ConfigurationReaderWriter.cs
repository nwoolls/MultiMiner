using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MultiMiner.Utility.Serialization
{
    public static class ConfigurationReaderWriter
    {
        public static T ReadConfiguration<T>(string fileName, string rootElement = null) where T : new()
        {
            if (File.Exists(fileName))
            {
                XmlSerializer serializer;
                if (String.IsNullOrEmpty(rootElement))
                    serializer = new XmlSerializer(typeof(T));
                else
                    serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));

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

        public static void WriteConfiguration(object source, string fileName, string rootElement = null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            Type type = source.GetType();

            XmlSerializer serializer;
            if (String.IsNullOrEmpty(rootElement))
                serializer = new XmlSerializer(type);
            else
                serializer = new XmlSerializer(type, new XmlRootAttribute(rootElement));

            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, source);
            }
        }
    }
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.Utility.Serialization
{
    public static class ConfigurationReaderWriter
    {
        private const string BackupExtension = ".bak";

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
                    catch (InvalidOperationException)
                    {
                        //close reader so we can delete the file
                        reader.Close();

                        result = HandleZeroByteConfiguration<T>(fileName, rootElement);
                    }

                    //backup settings
                    if (File.Exists(fileName))
                        BackupConfiguration(fileName);

                    return result;
                }
            }

            return new T();
        }

        private static void BackupConfiguration(string fileName)
        {
            string backupFileName = fileName + BackupExtension;
            File.Delete(backupFileName);
            File.Copy(fileName, backupFileName);
        }

        private static T HandleZeroByteConfiguration<T>(string fileName, string rootElement) where T : new()
        {
            string backupFileName = fileName + BackupExtension;

            T result;

            //file was 0 bytes - due to crash on startup
            if (File.Exists(backupFileName))
            {
                //restore backup file
                //MessageBox.Show(String.Format("The file {0} was 0 bytes (likely due to a crash on startup).\n\nA previous backup will be restored.",
                //    Path.GetFileName(fileName)), "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                File.Delete(fileName);
                File.Copy(backupFileName, fileName);

                //delete the backup file
                //reasoning: if the backup file is itself corrupt (0 bytes) it will keep being restored on
                //startup, showing the Warning dialog, unless the user makes some setting changes
                File.Delete(backupFileName);

                result = ConfigurationReaderWriter.ReadConfiguration<T>(fileName, rootElement);
            }
            else
            {
                //load defaults
                //MessageBox.Show(String.Format("The file {0} was 0 bytes (likely due to a crash on startup).\n\nDefault settings for this file will be loaded.",
                //    Path.GetFileName(fileName)), "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //delete the corrupt file
                //reasoning: if the user does not make some setting changes, the error will persist on startup
                File.Delete(fileName);

                result = new T();
            }

            return result;
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

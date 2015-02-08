using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiMiner.Utility.Serialization
{
    public class ObjectLogger
    {
        private const int MaxFileSize = 1 * 1000 * 1000; //1mb
        private readonly bool rollOverFiles;
        private readonly int oldFileSets;

        public ObjectLogger(bool rollOverFiles, int oldFileSets)
        {
            this.rollOverFiles = rollOverFiles;
            this.oldFileSets = oldFileSets;
        }

        //dates will be in UTC after loading
        public static IEnumerable<T> LoadLogFile<T>(string logFilePath)
        {
            string[] logFile = File.ReadAllLines(logFilePath);
            List<T> result = new List<T>();            
            foreach (string line in logFile)
            {
                result.Add(JsonConvert.DeserializeObject<T>(line));
            }
            return result;
        }

        public bool LogObjectToFile(Object objectToLog, string logFilePath)
        {
            RollOverLogFile(logFilePath);

            string jsonData = JsonConvert.SerializeObject(objectToLog);

            try
            {
                File.AppendAllText(logFilePath, jsonData + Environment.NewLine);
            }
            catch (Exception ex)
            {
                if ((ex is IOException) || (ex is UnauthorizedAccessException))
                {
                    //System.IO.IOException: The process cannot access the file 'ABC' because it is being used by another process.
                    //System.UnauthorizedAccessException: Access to the path 'ABC' is denied.
                    return false;
                }
                throw;
            }

            return true;
        }

        private void RollOverLogFile(string logFilePath)
        {
            if (rollOverFiles && File.Exists(logFilePath))
            {
                FileInfo fileInfo = new FileInfo(logFilePath);
                if (fileInfo.Length > MaxFileSize)
                {
                    BackupLogFileToSets(logFilePath);
                    File.Delete(logFilePath);
                }
            }
        }
        
        private void BackupLogFileToSets(string logFilePath)
        {
            if (oldFileSets > 0)
            {
                string backupFilePath;
                try
                {
                    for (int i = oldFileSets; i > 0; i--)
                    {
                        backupFilePath = Path.ChangeExtension(logFilePath, i.ToString());
                        string previousFilePath = Path.ChangeExtension(logFilePath, (i - 1).ToString());
                        if (File.Exists(backupFilePath))
                            File.Delete(backupFilePath);
                        if (File.Exists(previousFilePath))
                            File.Move(previousFilePath, backupFilePath);
                    }

                    backupFilePath = Path.ChangeExtension(logFilePath, "1");
                    if (File.Exists(backupFilePath))
                        File.Delete(backupFilePath);
                    File.Move(logFilePath, backupFilePath);
                }
                catch (UnauthorizedAccessException)
                {
                    //users report occasionally receiving access denied rolling log files
                    //have received this myself as well - may be in-use by an async log op
                    //no way to recover, cannot move file either (same exception)
                    //we'll try again on next call to the method
                }
            }
        }
    }
}

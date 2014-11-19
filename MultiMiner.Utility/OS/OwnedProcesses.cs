using MultiMiner.Utility.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MultiMiner.Utility.OS
{
    public class OwnedProcess
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string Name { get; set; }
    }

    public class OwnedProcesses
    {
        public static void SaveOwnedProcesses(IEnumerable<Process> processes, string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            List<OwnedProcess> children = new List<OwnedProcess>();
            foreach (Process process in processes)
            {
                try
                {
                    OwnedProcess child = new OwnedProcess()
                    {
                        Id = process.Id,
                        //no SessionId on Unix
                        SessionId = OSVersionPlatform.GetGenericPlatform() == System.PlatformID.Unix ? 0 : process.SessionId,
                        Name = process.ProcessName
                    };
                    children.Add(child);
                }
                catch (InvalidOperationException)
                {
                    //System.InvalidOperationException: Process has exited, so the requested information is not available.
                    //e.g. closing the application while SaveOwnedProcesses() is running
                }
            }
            ConfigurationReaderWriter.WriteConfiguration(children, fileName);
        }

        public static IEnumerable<Process> GetOwnedProcesses(string fileName)
        {
            if (!File.Exists(fileName))
                return new List<Process>();

            List<OwnedProcess> children = ConfigurationReaderWriter.ReadConfiguration<List<OwnedProcess>>(fileName);

            Process[] allProcesses = Process.GetProcesses();
            IEnumerable<Process> ownedProcesses = allProcesses.Where(
                ap => children.Any(
                    cp => (cp.Id == ap.Id)
                        && ((OSVersionPlatform.GetGenericPlatform() == System.PlatformID.Unix)
                            //no SessionId on Unix
                            || (cp.SessionId == ap.SessionId))
                        && (cp.Name.Equals(ap.ProcessName))                        
                )
            );

            return ownedProcesses;
        }
    }
}

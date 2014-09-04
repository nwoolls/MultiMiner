using MultiMiner.Utility.Serialization;
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
                OwnedProcess child = new OwnedProcess() 
                { 
                    Id = process.Id, 
                    SessionId = process.SessionId,
                    Name = process.ProcessName 
                };
                children.Add(child);        	
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
                        && (cp.SessionId == ap.SessionId)
                        && (cp.Name.Equals(ap.ProcessName))                        
                )
            );

            return ownedProcesses;
        }
    }
}

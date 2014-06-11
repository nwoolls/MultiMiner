using MultiMiner.Services;
using MultiMiner.Xgminer.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiMiner.Edge
{
    public class HardwareScanner
    {

        public async Task<object> GetDevices(object input)
        {
            DevicesService devicesService = new DevicesService();

            string workingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            List<Device> devices = devicesService.GetDevices(Path.Combine(workingPath,"Miners", "bfgminer", "bfgminer.exe"));

            return JsonConvert.SerializeObject(devices);
        }
    }
}

using System;

namespace MultiMiner.Xgminer.Api.Data
{
    public class VersionInformation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MinerVersion { get; set; }
        public string ApiVersion { get; set; }

        public VersionInformation()
        {
            Name = String.Empty;
            Description = String.Empty;
            MinerVersion = String.Empty;
            ApiVersion = String.Empty;
        }
    }
}

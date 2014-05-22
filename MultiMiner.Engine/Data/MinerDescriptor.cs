namespace MultiMiner.Engine.Data
{
    public class MinerDescriptor : AvailableMiner
    {
        public string FileName { get; set; }
        public bool LegacyApi { get; set; }
    }
}

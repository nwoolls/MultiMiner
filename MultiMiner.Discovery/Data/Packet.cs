namespace MultiMiner.Discovery.Data
{
    class Packet
    {
        public string MachineName { get; set; }
        public int Fingerprint { get; set; }
        public string Verb { get; set; }
    }
}

namespace MultiMiner.Discovery
{
    class Packet
    {
        public string MachineName { get; set; }
        public int Fingerprint { get; set; }
        public string Verb { get; set; }
    }
}

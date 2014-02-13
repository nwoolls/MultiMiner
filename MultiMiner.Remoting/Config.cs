namespace MultiMiner.Remoting
{
    public class Config
    {
        private const int UserPortMin = 49152;

        public const int BroadcastPort = UserPortMin + 1475;
        public const int RemotingPort = UserPortMin + 1473;
    }
}

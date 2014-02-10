namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    public class Path
    {
        public string SharedConfigPath { get; set; }
    }
}

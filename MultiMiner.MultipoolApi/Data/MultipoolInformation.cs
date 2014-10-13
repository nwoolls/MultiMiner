namespace MultiMiner.MultipoolApi.Data
{
    public class MultipoolInformation
    {
        //full algorithm name
        public string Algorithm { get; set; }
        //price in BTC / GH
        public double Price { get; set; }
        //normalize profit - calculated / estimated
        public double Profitability { get; set; }
    }
}

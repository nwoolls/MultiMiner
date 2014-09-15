namespace MultiMiner.ExchangeApi.Data
{
    public class ExchangeInformation
    {
        // Source is assumed to be 1.0
        // Target is the non-1.0 value
        public string SourceCurrency { get; set; }
        public string TargetSymbol { get; set; }
        public string TargetCurrency { get; set; }
        public double ExchangeRate { get; set; }
    }
}

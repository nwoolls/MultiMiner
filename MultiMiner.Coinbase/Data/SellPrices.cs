using System.Collections.Generic;

namespace MultiMiner.Coinbase.Data
{
    //https://coinbase.com/api/v1/prices/sell
    public class SellPrices : CurrencyAmount
    {
        public CurrencyAmount Subtotal { get; set; }
        public List<Dictionary<string, CurrencyAmount>> Fees { get; set; }
        public CurrencyAmount Total { get; set; }
    }
}
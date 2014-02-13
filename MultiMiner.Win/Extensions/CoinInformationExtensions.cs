using MultiMiner.CoinApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Win.Extensions
{
    public static class CoinInformationExtensions
    {
        private static class DogeSymbols
        {
            public const string Symbol1 = "DOG";
            public const string Symbol2 = "DOGE";
        }

        public static CoinInformation GetCoinInformationForSymbol(this List<CoinInformation> coinList, string coinSymbol)
        {
            CoinInformation info = coinList.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));

            //handling for DOGE and DOG both existing for Dogecoin
            //if this becomes an issue with more coins we can make this a bit nicer
            //simplicity wins for now
            if ((info == null) &&
                (coinSymbol.Equals(DogeSymbols.Symbol1, StringComparison.OrdinalIgnoreCase)
                || coinSymbol.Equals(DogeSymbols.Symbol2, StringComparison.OrdinalIgnoreCase)))
            {
                if (coinSymbol.Equals(DogeSymbols.Symbol1, StringComparison.OrdinalIgnoreCase))
                    coinSymbol = DogeSymbols.Symbol2;
                else
                    coinSymbol = DogeSymbols.Symbol1;
                info = coinList.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            }
            return info;
        }
    }
}

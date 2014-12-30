using System.Collections.Generic;

namespace MultiMiner.WhatToMine.Data
{
    public class ApiCoinInformation
    {
        public string Tag;
        public string Algorithm;
        public double Difficulty;
        public double Exchange_Rate;
        public int Profitability;
        public int Profitability24;
        public long Last_Block;
        public double Block_Reward;
        public double Nethash;
    }

    class ApiResponse
    {
        public Dictionary<string, ApiCoinInformation> Coins;
    }
}

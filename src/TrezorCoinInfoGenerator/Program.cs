﻿using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace TrezorCoinInfoGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            foreach (var fileName in Directory.GetFiles(@"C:\GitRepos\trezor-mcu\vendor\trezor-common\defs\bitcoin").Where(f => Path.GetExtension(f).ToLower() == ".json"))
            {
                var text = File.ReadAllText(fileName);
                var coinInfo = JsonConvert.DeserializeObject<CoinInfo>(text);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneratePrices
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("D:\\TextDebug\\allpnc.txt").Where(x => !string.IsNullOrWhiteSpace(x));
            var rd = new Random(100);

            var listCurrencies = new List<string>()
            {
                "AUD",
                "NZD"
            };

            foreach (var currency in listCurrencies)
            {
                var listProducts = new List<string>();

                foreach (var line in lines)
                {
                    var priceDO = (double)rd.Next(10_000, 100_000) / 100;
                    var priceDF = priceDO + (double)rd.Next(1_000, 10_000) / 100;


                    var priceDOStr = string.Format("{0:0.00}", priceDO);
                    var priceDFStr = string.Format("{0:0.00}", priceDF);


                    var tempLine =
                        $"{{\"priceType\":\"DO\",\"productID\":\"{line}\",\"priceAmount\":{priceDOStr},\"currencyCode\":\"{currency}\",\"validFrom\":\"2024-01-01\",\"validTo\":\"2024-12-31\"}},{{\"priceType\":\"DF\",\"productID\":\"{line}\",\"priceAmount\":{priceDFStr},\"currencyCode\":\"{currency}\",\"validFrom\":\"2024-01-01\",\"validTo\":\"2024-12-31\"}},";

                    listProducts.Add(tempLine);
                }

                var fileName = "GeneratePrices";
                File.WriteAllLines($"D:\\TextDebug\\{fileName}-{currency}.json", listProducts);
            }
        }
    }
}

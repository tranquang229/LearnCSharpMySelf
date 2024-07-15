// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using ConsoleApp3;

var lines = File.ReadAllLines("D:\\TextDebug\\IndexProduct-27-12-2023 1444.txt");
//var states = new List<string>()
//{
//    "AU02",
//    "AU03",
//    "AU04",
//    "AU05",
//    "AU06",
//    "AU08",
//};
var states = new List<string>()
{
    "NZ01",
    "NZ02"
};
var result = new List<string>();

foreach (var line in lines)
{
    foreach (var state in states)
    {
        string text = $"{line},{line}-ABC123,3.02E+12,NO,{DateTimeExtension.ConvertDateTimeToString(DateTimeExtension.RandomDate())},AUD2CDO,{NumberExtension.RandomNumber(100,1000)},0,0,0,0,{state},A";
        result.Add(text);
    }
}
File.WriteAllLines($"D:\\TextDebug\\Stocks-all-NZ.txt", result);


//var currency = "AUD";
//foreach (var line in lines)
//{
//    var dfPrice = NumberExtension.RandomNumber(100, 300);
//    var doPrice = dfPrice + 50;

//        string text = $"{{\"priceType\": \"DO\",\"productID\": \"{line}-ABC123\",\"priceAmount\": {doPrice},\"currencyCode\": \"{currency}\",\"validFrom\": \"2023-07-01\",\"validTo\": \"2024-12-31\"}},{{\"priceType\": \"DF\",\"productID\": \"{line}-ABC123\",\"priceAmount\": {dfPrice},\"currencyCode\": \"{currency}\",\"validFrom\": \"2023-07-01\",\"validTo\": \"2024-12-31\"}},";
//        result.Add(text);
//}

//File.WriteAllLines($"D:\\TextDebug\\Price-{currency}.txt", result);
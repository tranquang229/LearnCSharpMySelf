using Humanizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

            //string singular = "car";
            //string plural = pluralizationService.Pluralize(singular);
            //string singularized = pluralizationService.Singularize(plural);

            //Console.WriteLine($"Singular: {singular}");
            //Console.WriteLine($"Plural: {plural}");
            //Console.WriteLine($"Singularized: {singularized}");
            //string singleWorld = Pluralization.Singularize(word);
            //Console.WriteLine(singleWorld);
            string singular = "Cactus";
            string plural = singular.Pluralize();
            string singularized = plural.Singularize();

        }
    }
}

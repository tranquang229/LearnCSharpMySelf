using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines($"D:\\TextDebug\\Errors.txt");

            var fileNames = new List<string>();
           
            Regex rx = new Regex(@"0>(.*?)\(\d");
            foreach (var line in lines)
            {
                var fileName = rx.Match(line).Groups[1].Value;
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileNames.Add(fileName);
                }
            }
           
            File.WriteAllLines($"D:\\TextDebug\\Errors--Result.txt", fileNames);

        }
    }

    public class PlantToStateModel
    {
        [Display(Name = "Plant Code", Description = "Plant Code")]
        public string PlantCode { get; set; }

        [Display(Name = "Plant Description", Description = "Plant Description")]
        public string PlantDescription { get; set; }

        [Display(Name = "State Code", Description = "State Code")]
        public string StateCode { get; set; }

        [Display(Name = "State Description", Description = "State Description")]
        public string StateDescription { get; set; }
    }
}

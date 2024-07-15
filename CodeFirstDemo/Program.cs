using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeFirstDemo
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CourseLevel Level { get; set; }
        public float FullPrice { get; set; }
        public Author Author { get; set; }
        public IList<Tag> Tags { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; }
    }

    public enum CourseLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }

    public class PlutoContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public PlutoContext()
            : base("name=DefaultConnection")
        {
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            //// string orderLineStatus = "InProgress";
            ////var x = Enum.Parse(typeof(SapOrderLineStatus), orderLineStatus, true);
            //var check1 = CanParseToInteger("1.0");
            //var check2 = CanParseToInteger("1.00");
            //var check3 = CanParseToInteger("1");
            //var check4 = CanParseToInteger("-5.6");
            //var check5 = CanParseToInteger("211");
            //var check6 = CanParseToInteger("0");
            //SapOrderLineStatus operation = (SapOrderLineStatus)Enum.Parse(typeof(SapOrderLineStatus), "In_Progress");
            //var strInput = "in progress ";
            //var result = MapOrderLineStatusToOrderLineCode(strInput);
            //var x = 0;
            //var test = (SapOrderLineStatus)x;
            //strInput = strInput
            //    .Replace("-", "")
            //    .Replace("_", "")
            //    .Replace(" ", "");

            //if (Enum.TryParse<SapOrderLineStatus>(strInput, true, out var res))
            //{
            //    Console.WriteLine("test");
            //}
            //else
            //{
            //    Console.WriteLine("test2");
            //}
            var fileName = "PartialPushstock20240901042845CustomerAUD2CDO_NITECO";

            var dtFormat = "yyyyddMMHHmmss";
            var dt = ExtractDateTimeFromFileName(fileName, dtFormat);
IPluralizationService
            PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

        }

        public static string SingularizeWord(string wordToSingularize)
        {
            // Only make it singular if the culture is english
            CultureInfo culture = new CultureInfo("en");
            var service = PluralizationService.CreateService(culture);
            StringBuilder singularized = new StringBuilder();
            string[] words = wordToSingularize.Split(' ');
            string separator = string.Empty;

            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word) && word.Length > 1)
                {
                    singularized.Append(separator + service.Singularize(word));
                }
                else
                {
                    singularized.Append(separator + word);
                }

                separator = " ";
            }

            return singularized.ToString();
        }


        public static DateTime ExtractDateTimeFromFileName(string fileName, string datetimeFormat)
        {
            try
            {
                var fileNameReplaced = fileName.Replace("_", "");
                var datetimeFormatReplaced = datetimeFormat.Replace("_", "");
                var lengthOfTimestamp = string.IsNullOrEmpty(datetimeFormat)
                    ? 14
                    : datetimeFormatReplaced.Length;
                var pattern = $"\\d{{{lengthOfTimestamp}}}";
                var date = Regex.Match(fileNameReplaced, pattern).Value;

                return DateTime.ParseExact(date, datetimeFormatReplaced, CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public static int MapOrderLineStatusToOrderLineCode(string orderLineStatus)
        {
            try
            {
                orderLineStatus = orderLineStatus
                    .Replace("-", "")
                    .Replace("_", "")
                    .Replace(" ", "");

                if (Enum.TryParse<SapOrderLineStatus>(orderLineStatus, true, out var res))
                {
                    return (int)res;
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            return default;
        }



        public enum SapOrderLineStatus
        {
            [Display(Name = "Accepted")]
            Accepted = 520,

            [Display(Name = "In Progress")]
            InProgress = 540,

            [Display(Name = "Shipped")]
            Shipped = 560,

            [Display(Name = "Completed")]
            Completed = 580,

            [Display(Name = "Cancelled")]
            Cancelled = 980
        }





        //public static T ParseOrDefault<T>(string input, T @default = default)
        //{
        //    object res;
        //    if (Enum.TryParse(typeof(T), input, out res))
        //        return (T)res;
        //    return @default;
        //}





        public static bool CanParseToInteger(object obj)
        {
            try
            {
                var decimalValue = decimal.Parse(obj.ToString());
                return decimalValue % 1 == 0;
            }
            catch (Exception e)
            {
                // Ignore
            }
            return false;
        }
    }

    public enum SapOrderLineStatus
    {
        [Display(Name = "Accepted")]
        Accepted = 520,

        [Display(Name = "In Progress")]
        InProgress = 540,

        [Display(Name = "Shipped")]
        Shipped = 560,

        [Display(Name = "Completed")]
        Completed = 580,

        [Display(Name = "Cancelled")]
        Cancelled = 980
    }
}
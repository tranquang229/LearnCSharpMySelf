using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestParalle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParallelDump();

            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(i);
                }
            });

            Console.ReadLine();
        }

        static void ParallelDump()
        {
            var listInt = new List<int>()
            {
                1,2, 3, 4, 5, 6, 7, 8, 9, 10
            };

            Parallel.ForEach(listInt, new ParallelOptions { MaxDegreeOfParallelism = 3 },
                p =>
                {
                    Console.WriteLine($"-----{p}");
                    Thread.Sleep(3000);
                });
        }
    }
}

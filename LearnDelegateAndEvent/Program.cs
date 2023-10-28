using System;

namespace LearnDelegateAndEvent
{
    public delegate void ShowLog(string message);
    internal class Program
    {
        static void Main(string[] args)
        {
            //ShowLog showLog = null;
            //showLog += Info;
            //showLog += Warning;
            //showLog?.Invoke("Chào moi nguoi");
            Action<string> action1 = Info;
            action1("Xin chao moi nguoi");

            Func<int, int, int> tinhToan = TinhTong;
            int a = 5;
            int b = 10;
            tinhToan = TinhTong;
            Console.WriteLine($"Ket qua la ${tinhToan.Invoke(a, b)}");
        }

        private static int TinhTong(int a, int b)
        {
            return a + b;
        }
        private static int TinhHieu(int a, int b)
        {
            return a - b;
        }

        public static void Info(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ResetColor();
        }

        public static void Warning(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ResetColor();
        }

    }
}

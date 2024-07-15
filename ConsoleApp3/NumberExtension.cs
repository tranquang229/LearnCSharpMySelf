namespace ConsoleApp3
{
    public static class NumberExtension
    {
        public static int RandomNumber(int min, int max)
        {
            Random rd = new Random();

            var result = rd.Next(min, max);

            return result;
        }
    }
}

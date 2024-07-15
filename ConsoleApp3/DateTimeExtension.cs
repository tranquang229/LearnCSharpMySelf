namespace ConsoleApp3
{
    public  class DateTimeExtension
    {
        public static DateTime RandomDate()
        {
            Random rnd = new Random();
            // Define the minimum and maximum datetimes
            DateTime minDate = DateTime.Now.AddMonths(1); // current datetime
            DateTime maxDate = DateTime.Now.AddYears(10); // 10 years from now

            // Calculate the difference between the two datetimes
            TimeSpan timeSpan = maxDate - minDate;

            // Generate a random fraction from 0 to 1
            double fraction = rnd.NextDouble();

            // Multiply the fraction by the total seconds in the timespan
            double seconds = fraction * timeSpan.TotalSeconds;

            // Add the random seconds to the minimum datetime
            DateTime randomDate = minDate.AddSeconds(seconds);

            // Print the result
            return randomDate;
        }

        public static string ConvertDateTimeToString(DateTime dateTime)
        {
            var result = dateTime.ToString("dd/MM/yyyy");
            return result;
        }
    }
}

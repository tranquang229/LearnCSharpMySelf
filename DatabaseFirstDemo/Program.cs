using System;

namespace DatabaseFirstDemo
{
    public enum Level : byte
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dbContext = new PlutoDbContext();
            var courses = dbContext.GetCourses();
            //var course = dbContext.GetAuthorCourses(1);
            var course = new Course();
            course.Level = Level.Beginner;
            foreach (var c in courses)
            {
                Console.WriteLine(c.Title);
            }
        }
    }
}
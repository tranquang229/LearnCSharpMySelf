using System.Collections.Generic;

namespace CodeFirstExistingDatabase.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CodeFirstExistingDatabase.PlutoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CodeFirstExistingDatabase.PlutoContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            context.Authors.AddOrUpdate(a => a.Name,
                new Author()
                {
                    Name = "Author 5",
                    Courses = new List<Course>()
                    {
                        new Course() {Name="Course for Author 5 new", Description = "Description 5"}
                    }
                });
        }
    }
}
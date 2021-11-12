using CourseLibrary.Api.Models.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options)
            :base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<Author>().HasData(
                new Author()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Berry",
                    LastName = "Griffin Beak Eldritch",
                    DateOfBirth = new DateTime(1650, 7, 23),
                    MainCategory = "Ships"
                },
                new Author()
                {
                    Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    FirstName = "Nancy",
                    LastName = "Swashbuckler Rye",
                    DateOfBirth = new DateTime(1668, 5, 21),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    FirstName = "Eli",
                    LastName = "Ivory Bones Sweet",
                    DateOfBirth = new DateTime(1701, 12, 16),
                    MainCategory = "Singing"
                },
                new Author()
                {
                    Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                    FirstName = "Arnold",
                    LastName = "The Unseen Stafford",
                    DateOfBirth = new DateTime(1702, 3, 6),
                    MainCategory = "Singing"
                },
                new Author()
                {
                    Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                    FirstName = "Seabury",
                    LastName = "Toxic Reyson",
                    DateOfBirth = new DateTime(1690, 11, 23),
                    MainCategory = "Maps"
                },
                new Author()
                {
                    Id = Guid.Parse("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                    FirstName = "Rutherford",
                    LastName = "Fearless Cloven",
                    DateOfBirth = new DateTime(1723, 4, 5),
                    MainCategory = "General debauchery"
                },
                new Author()
                {
                    Id = Guid.Parse("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                    FirstName = "Atherton",
                    LastName = "Crow Ridley",
                    DateOfBirth = new DateTime(1721, 10, 11),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("4b822af2-4a03-40c4-bea7-62aade333b32"),
                    FirstName = "Sadek",
                    LastName = "Mohamed",
                    DateOfBirth = new DateTime(1999, 10, 10),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("1505dc65-0594-4e1e-98fb-49fb19e09db2"),
                    FirstName = "Salem",
                    LastName = "Ahmed",
                    DateOfBirth = new DateTime(1993, 10, 10),
                    MainCategory = "Ships"
                },
                new Author()
                {
                    Id = Guid.Parse("0f7ab3e2-dc5c-4512-93d0-cea929ca7508"),
                    FirstName = "Mohamed",
                    LastName = "Shaaban",
                    DateOfBirth = new DateTime(2000, 10, 10),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("6508b058-31fd-43da-890f-14fe8ed5f290"),
                    FirstName = "Reda",
                    LastName = "Mabrook",
                    DateOfBirth = new DateTime(1970, 10, 10),
                    MainCategory = "Rum"
                },
                new Author()
                {
                    Id = Guid.Parse("1279039b-665d-4cc3-b0d7-d3c643878f84"),
                    FirstName = "Haitham",
                    LastName = "Mohsen",
                    DateOfBirth = new DateTime(1975, 10, 10),
                    MainCategory = "Ships"
                },
                new Author()
                {
                    Id = Guid.Parse("b2193e38-2226-4985-91a0-57a535a9f0e5"),
                    FirstName = "Mohamed",
                    LastName = "Amin",
                    DateOfBirth = new DateTime(1990, 10, 10),
                    MainCategory = "Ships"
                },
                new Author()
                {
                    Id = Guid.Parse("66adbead-f3a5-4558-92b2-f88301b067e3"),
                    FirstName = "Mosh",
                    LastName = "Hamedani",
                    DateOfBirth = new DateTime(1990, 10, 10),
                    MainCategory = "Ships"
                },
                 new Author()
                 {
                     Id = Guid.Parse("4b1111bb-a637-4414-aba5-872a944a9fa1"),
                     FirstName = "Tim",
                     LastName = "Corey",
                     DateOfBirth = new DateTime(1990, 10, 10),
                     MainCategory = "Rum"
                 },
                 new Author()
                 {
                     Id = Guid.Parse("17f93f58-ca1b-4064-aaba-2072185ec130"),
                     FirstName = "Amr",
                     LastName = "Gaber",
                     DateOfBirth = new DateTime(1985, 10, 10),
                     MainCategory = "Rum"
                 },
                  new Author()
                  {
                      Id = Guid.Parse("72cf36f7-8f10-4ab4-9a92-cf7c5c1b6e29"),
                      FirstName = "Eslam",
                      LastName = "Gaber",
                      DateOfBirth = new DateTime(1985, 10, 10),
                      MainCategory = "Rum"
                  },
                   new Author()
                   {
                       Id = Guid.Parse("153552a4-cb9c-4566-8040-8d44cb7428f8"),
                       FirstName = "Ahmed",
                       LastName = "Wael",
                       DateOfBirth = new DateTime(1985, 10, 10),
                       MainCategory = "Rum"
                   },
                   new Author()
                   {
                       Id = Guid.Parse("93fe5856-6806-4b27-b1bd-8b98599ca6bd"),
                       FirstName = "Toty",
                       LastName = "El-Tablawy",
                       DateOfBirth = new DateTime(1985, 10, 10),
                       MainCategory = "Love"
                   },
                   new Author()
                   {
                       Id = Guid.Parse("11e09402-ffd8-4ef8-853e-35d8950c70ae"),
                       FirstName = "Bosy",
                       LastName = "El-Tablawy",
                       DateOfBirth = new DateTime(1985, 10, 10),
                       MainCategory = "Ships"
                   }
                );

            modelBuilder.Entity<Course>().HasData(
               new Course
               {
                   Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Commandeering a Ship Without Getting Caught",
                   Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
               },
               new Course
               {
                   Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                   AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Overthrowing Mutiny",
                   Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
               },
               new Course
               {
                   Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                   AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                   Title = "Avoiding Brawls While Drinking as Much Rum as You Desire",
                   Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk."
               },
               new Course
               {
                   Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                   AuthorId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                   Title = "Singalong Pirate Hits",
                   Description = "In this course you'll learn how to sing all-time favourite pirate songs without sounding like you actually know the words or how to hold a note."
               }
               );
            base.OnModelCreating(modelBuilder);
        }
    }
}

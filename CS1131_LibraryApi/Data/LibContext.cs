using System;
using CS1131_LibraryApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace CS1131_LibraryApi.Data
{
    public class LibContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }

        //https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
        public LibContext(DbContextOptions<LibContext> options): base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author()
                {
                    Id = -1,
                    FirstName = "Umberto",
                    LastName = "Eco"
                },
                new Author()
                {
                    Id = -2,
                    FirstName = "Haruki",
                    LastName = "Murakami"
                }
            );

            modelBuilder.Entity<Member>().HasData(
                new Member()
                {
                    Id = -1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "jsmith@example.com",
                });

            modelBuilder.Entity<Book>().HasData(
                new Book()
                {
                    Id = -1,
                    Name = "The Name of the Rose",
                    Publisher = "Fixed House",
                    AuthorId = -1, 
                    RentedToId = -1
                },
                new Book()
                {
                    Id = -2,
                    Name = "The Limits of Interpretation",
                    Publisher = "Fixed House",
                    AuthorId = -1
                },
                new Book()
                {
                    Id = -3,
                    Name = "Kafka on the Shore",
                    Publisher = "Arctic Editions",
                    AuthorId = -2
                });


        }
    }
}

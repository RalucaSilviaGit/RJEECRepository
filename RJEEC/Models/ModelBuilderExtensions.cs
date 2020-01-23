using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Author>().HasData(
            //    new Author() { Id = 1, FirstName = "Mary", LastName = "Poppins", Email = "mary@pragimtech.com" },
            //    new Author() { Id = 2, FirstName = "John", LastName = "IT", Email = "john@pragimtech.com" },
            //    new Author() { Id = 3, FirstName = "Sam", LastName = "IT", Email = "sam@pragimtech.com" }
            //);

            //modelBuilder.Entity<Event>().HasData(
            //    new Event() { Id = 1, Name = "Event1", Date = new DateTime(), Description = "Event mary@pragimtech.com" },
            //    new Event() { Id = 2, Name = "Event2", Date = new DateTime(), Description = "Event john@pragimtech.com" },
            //    new Event() { Id = 3, Name = "Event3", Date = new DateTime(), Description = "Event sam@pragimtech.com" }
            //);
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class RJEECDbContext : IdentityDbContext
    {
        public RJEECDbContext(DbContextOptions<RJEECDbContext> options) : base(options)
        {

        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventPhoto> EventPhotos { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Magazine> Magazines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.Seed();
        }
    }
}

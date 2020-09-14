using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
            //modelBuilder.Entity<Event>().HasMany(b => b.EventPhotos).WithOne()
            //    .HasForeignKey(b => b.EventId)
            //    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Seed();
        }
    }
}

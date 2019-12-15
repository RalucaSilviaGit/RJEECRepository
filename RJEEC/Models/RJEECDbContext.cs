using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class RJEECDbContext : DbContext
    {
        public RJEECDbContext(DbContextOptions<RJEECDbContext> options) : base(options)
        {

        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}

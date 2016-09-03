using System.Data.Entity;
using JulJul.Core.Domain;

namespace JulJul.Repository.Ef6
{
    public class EfDbContext:DbContext
    {
        public EfDbContext() :base("JulJulConnectionString")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
    }
}

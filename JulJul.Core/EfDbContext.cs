using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Domain;

namespace JulJul.Core
{
    public class EfDbContext:DbContext
    {
        public EfDbContext() :base("JulJulConnectionString")
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}

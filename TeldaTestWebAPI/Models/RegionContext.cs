using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TeldaTestWebAPI.Models
{
    public class RegionContext : DbContext
    {

            public DbSet<Region> Region { get; set; }
            public RegionContext(DbContextOptions<RegionContext> options)
                : base(options)
            {
                Database.EnsureCreated();
            }
    }
}

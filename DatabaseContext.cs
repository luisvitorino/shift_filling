using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR
{
    public class DatabaseContext : DbContext 
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        public DbSet<GR.Models.Gage_Demand> Gage_Demand { get; set; }

        public DbSet<GR.Models.Gage_Capacity> Gage_Capacity { get; set; }

        public DbSet<GR.Models.Gage_Curing> Gage_Curing { get; set; }

        public DbSet<GR.Models.Gage_Straingage> Gage_Straingage { get; set; }

        public DbSet<GR.Models.Gage_Schedule> Gage_Schedule { get; set; }
    }
}

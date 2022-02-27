using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Biosim.Models
{
    public class EFContext : DbContext
    {
        private const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=BioSimDatabase;Trusted_Connection=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<HerbivoreModel> Herbivores { get; set; } // Collection of all dead herbivores
        public DbSet<CarnivoreModel> Carnivores { get; set; } // Collection of all dead carnivores
        public DbSet<ResultModel> Results { get; set; }
    }
}

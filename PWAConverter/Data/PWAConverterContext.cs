using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using PWAConverter.Entities;
using System.Reflection.Emit;

namespace PWAConverter.Data
{
    public class PWAConverterContext : DbContext
    {
        public PWAConverterContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Manifest> Manifests { get; set; }
        public DbSet<Source> Sources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=localhost; Initial Catalog=PWAConverter; Integrated Security=true");
        }
       
    }
    }







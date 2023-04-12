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
            optionsBuilder.UseSqlServer(@"Server=tcp:pwaconverter.database.windows.net,1433;Initial Catalog=PWAConverter;Persist Security Info=False;User ID=pwadbserver;Password=bitirmetezibitartik2023__;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Database=PWAConverter;");
        }
       
    }
    }







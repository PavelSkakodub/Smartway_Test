using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Smartway_Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smartway_Test.Controllers
{
    public class AppDbContext : DbContext
    {
        public DbSet<File> Files { get; set; }
        public DbSet<Link> Links { get; set; }
        private IConfiguration Configuration { get; }

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public AppDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=Smartway;Trusted_Connection=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new FilesConfiguration());
            builder.Entity<Link>().Property(x => x.Hash).HasMaxLength(30);
        }

        public class FilesConfiguration : IEntityTypeConfiguration<File>
        {
            public void Configure(EntityTypeBuilder<File> builder)
            {
                builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
                builder.Property(x => x.Size).IsRequired().HasDefaultValue(0);
                builder.Property(x => x.Format).HasMaxLength(10);

                builder
                    .HasMany(x => x.Links)
                    .WithOne(x => x.File)
                    .HasForeignKey(x => x.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}

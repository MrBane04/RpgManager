using Microsoft.EntityFrameworkCore;
using RpgManager.Core.Models;
namespace RpgManager.Core.Data
{
    public class RpgManagerDbContext : DbContext 
    {

        public DbSet<Campaign> Campaigns { get; set; }
        public RpgManagerDbContext(DbContextOptions<RpgManagerDbContext> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campaign>(entity =>
            {
                modelBuilder.Entity<Campaign>().ToTable("campaigns");
                modelBuilder.Entity<Campaign>()
                    .Property(c => c.Name)
                    .HasColumnName("name");
                modelBuilder.Entity<Campaign>()
                    .Property(c => c.System)
                    .HasColumnName("system");
                entity.Property(c => c.Status)
                    .HasColumnName("status")
                    .HasConversion<string>();
                modelBuilder.Entity<Campaign>()
                    .Property(c => c.Id)
                    .HasColumnName("id");

            });

        }
    }
}

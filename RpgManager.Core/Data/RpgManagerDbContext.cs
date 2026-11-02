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
                entity.ToTable("campaigns");

                entity.Property(c => c.Name)
                    .HasColumnName("name");

                entity.Property(c => c.System)
                    .HasColumnName("system");

                entity.Property(c => c.Status)
                    .HasColumnName("status")
                    .HasConversion<string>();

                entity.Property(c => c.Id)
                    .HasColumnName("id");

            });

            modelBuilder.Entity<Session>(entity => 
            { 
                entity.ToTable("sessions");

                entity.HasOne(s => s.Campaign)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CampaignId);

                entity.Property(s => s.Id)
                .HasColumnName("id");

                entity.Property(s => s.CampaignId)
                .HasColumnName("campaign_id");

                entity.Property(s => s.Title)
                .HasColumnName("title");

                entity.Property(s => s.Description)
                .HasColumnName("description");

                entity.Property(s => s.SessionDate)
                .HasColumnName("session_date");

                entity.Property(s => s.DurationMinutes)
                .HasColumnName("duration_minutes");

                entity.Property(s => s.Status)
                .HasColumnName("status")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<SessionStatus>(v,true));

                entity.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            });

        }

        public DbSet<Session> Sessions { get; set; }

        

    }
}

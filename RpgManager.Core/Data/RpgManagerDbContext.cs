using Microsoft.EntityFrameworkCore;
using RpgManager.Core.Models;
namespace RpgManager.Core.Data
{
    public class RpgManagerDbContext : DbContext 
    {

        public DbSet<Note> Notes { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }
        public RpgManagerDbContext(DbContextOptions<RpgManagerDbContext> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Character>(entity =>
            {
                modelBuilder.Entity<Note>(entity =>
                {
                    entity.ToTable("notes");

                    entity.HasOne(n => n.Campaign)
                        .WithOne(c => c.Note)
                        .HasForeignKey<Note>(n => n.CampaignId);

                    entity.Property(n => n.Id).HasColumnName("id");
                    entity.Property(n => n.CampaignId).HasColumnName("campaign_id");
                    entity.Property(n => n.Content)
                        .HasColumnName("content")
                        .HasDefaultValue("");
                    entity.Property(n => n.CreatedAt)
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("NOW()");
                    entity.Property(n => n.UpdatedAt)
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("NOW()");
                });

                entity.ToTable("characters");

                entity.HasOne(s => s.Campaign)
                .WithMany(c => c.Characters)
                .HasForeignKey(s => s.CampaignId);

                entity.Property(s => s.Id)
                .HasColumnName("id");

                entity.Property(s => s.CampaignId)
                .HasColumnName("campaign_id");

                entity.Property(s => s.Name)
                .HasColumnName("name");

                entity.Property(s => s.Age)
                .HasColumnName("age");

                entity.Property(s => s.Race)
                .HasColumnName("race");

                entity.Property(s => s.CharacterClass)
                .HasColumnName("character_class");

                entity.Property(s => s.Level)
                .HasColumnName("level");



                entity.Property(s => s.Description)
                .HasColumnName("description");

                entity.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");
            }
            );

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

        public DbSet<Character> Characters { get; set; }



    }
}

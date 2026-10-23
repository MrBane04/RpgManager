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
    }
}

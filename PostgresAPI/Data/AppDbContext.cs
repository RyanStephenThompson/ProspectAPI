using Microsoft.EntityFrameworkCore;
using PostgresAPI.Models;

namespace PostgresAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Prospect> prospects { get; set; }
        public DbSet<Reward> rewards { get; set; }
        public DbSet<Redemption> redemptions { get; set; }


    }
}

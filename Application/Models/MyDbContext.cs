using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;

namespace Demo.Models
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<VersionInfo> VersionInfo { get; set; }
        public DbSet<Audit> Audit { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}

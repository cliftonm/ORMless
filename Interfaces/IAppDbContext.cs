using Microsoft.EntityFrameworkCore;

using Models;

namespace Interfaces
{
    public interface IAppDbContext
    {
        DbSet<VersionInfo> VersionInfo { get; set; }
        DbSet<Audit> Audit { get; set; }
        int SaveChanges();
    }
}

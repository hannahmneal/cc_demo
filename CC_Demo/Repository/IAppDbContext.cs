using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Data;

public interface IAppDbContext
{
    DbSet<Creator> Creator { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

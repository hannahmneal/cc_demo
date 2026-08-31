using CC_Demo.Data;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public interface IAppDbContext
{
    DbSet<Creator> Creator { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

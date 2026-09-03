using CC_Demo.Models;
using CC_Demo.Models.Marvel;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public interface IAppDbContext
{
    DbSet<Creator> Creator { get; }
    DbSet<MarvelRecord> MarvelRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

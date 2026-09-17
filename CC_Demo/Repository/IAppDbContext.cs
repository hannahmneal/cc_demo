using CC_Demo.Models;
using CC_Demo.Models.Gcd;
using CC_Demo.Models.Marvel;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public interface IAppDbContext
{
    DbSet<Creator> Creator { get; }
    DbSet<MarvelRecord> MarvelRecords { get; }
    DbSet<GcdRecord> GcdRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

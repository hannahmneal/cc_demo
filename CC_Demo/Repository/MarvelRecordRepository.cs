using CC_Demo.Models.Marvel;
using CC_Demo.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public class MarvelRecordRepository : IMarvelRecordRepository
{
    private readonly IAppDbContext _db;

    public MarvelRecordRepository(IAppDbContext db) => _db = db;

    public Task<MarvelRecord?> GetByIdAsync(string resource, Ulid id, CancellationToken ct = default) =>
        _db.MarvelRecords.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Resource == resource && m.Id == id, ct);

    public Task<MarvelRecord?> GetByMarvelIdAsync(string resource, int marvelId, CancellationToken ct = default) =>
        _db.MarvelRecords.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Resource == resource && m.MarvelId == marvelId, ct);

    public Task<PagedResult<MarvelRecord>> GetAllAsync(string resource, PaginationRequest pagination, CancellationToken ct = default) =>
        _db.MarvelRecords.AsNoTracking()
            .Where(m => m.Resource == resource)
            .OrderBy(m => m.Id)
            .ToPagedResultAsync(pagination, ct);
}

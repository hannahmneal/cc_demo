using CC_Demo.Models;
using CC_Demo.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IAppDbContext _db;

    public CreatorRepository(IAppDbContext db) => _db = db;

    public Task<Creator?> GetByIdAsync(Ulid id, CancellationToken ct = default) =>
        _db.Creator.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Creator?> GetByMarvelIdAsync(int marvelId, CancellationToken ct = default) =>
        _db.Creator.FirstOrDefaultAsync(c => c.MarvelId == marvelId, ct);

    public Task<PagedResult<Creator>> GetAllAsync(PaginationRequest pagination, CancellationToken ct = default) =>
        _db.Creator.AsNoTracking()
            .OrderBy(c => c.Id)
            .ToPagedResultAsync(pagination, ct);

    public async Task AddAsync(Creator creator, CancellationToken ct = default)
    {
        await _db.Creator.AddAsync(creator, ct);
        await _db.SaveChangesAsync(ct);
    }
}

using CC_Demo.Data;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IAppDbContext _db;

    public CreatorRepository(IAppDbContext db) => _db = db;

    public Task<Creator?> GetByIdAsync(Ulid id, CancellationToken ct = default) =>
        _db.Creator.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Creator>> GetAllAsync(CancellationToken ct = default) =>
        _db.Creator.ToListAsync(ct);

    public async Task AddAsync(Creator creator, CancellationToken ct = default)
    {
        await _db.Creator.AddAsync(creator, ct);
        await _db.SaveChangesAsync(ct);
    }
}

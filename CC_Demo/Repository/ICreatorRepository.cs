using CC_Demo.Data;

namespace CC_Demo.Repository;

public interface ICreatorRepository
{
    Task<Creator?> GetByIdAsync(Ulid id, CancellationToken ct = default);
    Task<List<Creator>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Creator creator, CancellationToken ct = default);
}

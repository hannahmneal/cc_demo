namespace CC_Demo.Data;

public interface ICreatorRepository
{
    Task<Creator?> GetByIdAsync(Ulid id, CancellationToken ct = default);
    Task<List<Creator>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Creator creator, CancellationToken ct = default);
}

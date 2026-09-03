using CC_Demo.Models;
using CC_Demo.Models.Pagination;

namespace CC_Demo.Repository;

public interface ICreatorRepository
{
    Task<Creator?> GetByIdAsync(Ulid id, CancellationToken ct = default);
    Task<Creator?> GetByMarvelIdAsync(int marvelId, CancellationToken ct = default);
    Task<PagedResult<Creator>> GetAllAsync(PaginationRequest pagination, CancellationToken ct = default);
    Task AddAsync(Creator creator, CancellationToken ct = default);
}

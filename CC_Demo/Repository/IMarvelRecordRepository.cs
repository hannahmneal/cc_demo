using CC_Demo.Models.Marvel;
using CC_Demo.Models.Pagination;

namespace CC_Demo.Repository;

/// <summary>
/// Read-only access to raw <c>cc_demo</c> rows, scoped by Marvel resource type.
/// </summary>
public interface IMarvelRecordRepository
{
    Task<MarvelRecord?> GetByIdAsync(string resource, Ulid id, CancellationToken ct = default);

    Task<MarvelRecord?> GetByMarvelIdAsync(string resource, int marvelId, CancellationToken ct = default);
    Task<PagedResult<MarvelRecord>> GetAllAsync(string resource, PaginationRequest pagination, CancellationToken ct = default);
}

using CC_Demo.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public static class PaginationExtensions
{
    /// <summary>
    /// Applies <see cref="PaginationRequest"/> to an already-ordered query, pushing Skip/Take
    /// (or no limit, for "all") down to the database rather than paging an in-memory list.
    /// </summary>
    /// <param name="orderedQuery">A query with a deterministic ORDER BY already applied.</param>
    /// <param name="pagination">The requested page and page size.</param>
    /// <param name="ct">Cancellation token for the request.</param>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> orderedQuery,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var totalCount = await orderedQuery.CountAsync(ct);

        if (pagination.PageSize is null)
        {
            var all = await orderedQuery.ToListAsync(ct);
            return new PagedResult<T>(all, totalCount, 1, totalCount, totalCount == 0 ? 0 : 1);
        }

        var pageSize = pagination.PageSize.Value;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await orderedQuery
            .Skip((pagination.Page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>(items, totalCount, pagination.Page, pageSize, totalPages);
    }
}

namespace CC_Demo.Models.Pagination;

/// <summary>
/// A validated page number and page size. <see cref="PageSize"/> is <c>null</c> when the
/// caller asked for every result (<c>pageSize=all</c>).
/// </summary>
public record PaginationRequest(int Page, int? PageSize)
{
    public static readonly int[] AllowedPageSizes = [20, 50, 100];

    /// <summary>
    /// Parses and validates raw <c>page</c>/<c>pageSize</c> query values.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">One of "20", "50", "100", or "all" (case-insensitive).</param>
    /// <param name="request">The parsed request, if valid.</param>
    /// <param name="error">A validation error message, if invalid.</param>
    public static bool TryParse(int page, string pageSize, out PaginationRequest request, out string? error)
    {
        if (page < 1)
        {
            request = null!;
            error = "'page' must be 1 or greater.";
            return false;
        }

        if (string.Equals(pageSize, "all", StringComparison.OrdinalIgnoreCase))
        {
            request = new PaginationRequest(1, null);
            error = null;
            return true;
        }

        if (!int.TryParse(pageSize, out var size) || !AllowedPageSizes.Contains(size))
        {
            request = null!;
            error = $"'pageSize' must be one of {string.Join(", ", AllowedPageSizes)}, or 'all'.";
            return false;
        }

        request = new PaginationRequest(page, size);
        error = null;
        return true;
    }
}

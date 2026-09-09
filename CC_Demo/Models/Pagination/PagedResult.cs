namespace CC_Demo.Models.Pagination;

/// <summary>
/// A page of results, along with enough metadata for a client to know how many results
/// exist in total, which page it's looking at, and how many pages there are.
/// </summary>
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages)
{
    /// <summary>
    /// The plural key <see cref="Items"/> is serialized under (e.g. "creators"). Set by the
    /// controller per-response; consumed by <see cref="PagedResultJsonConverterFactory"/>, not serialized itself.
    /// </summary>
    public string ItemsName { get; init; } = "items";

    /// <summary>
    /// Projects the items in this page to a different shape while preserving all paging metadata.
    /// </summary>
    public PagedResult<TOut> Select<TOut>(Func<T, TOut> selector) =>
        new(Items.Select(selector).ToList(), TotalCount, Page, PageSize, TotalPages) { ItemsName = ItemsName };
}

namespace CC_Demo.Models.Gcd;

/// <summary>
/// A raw row from the <c>Raw_GCD_Data</c> table, as ingested from the Grand Comics Database
/// by an external (Python) scraper. Unlike <see cref="Marvel.MarvelRecord"/>'s <c>cc_demo</c> table,
/// this table's schema is owned by EF Core migrations even though only the external scraper writes
/// rows into it, so the raw shape stays under version control here rather than drifting silently.
/// <see cref="Resource"/> distinguishes which GCD entity type (e.g. series, issue, creator, publisher)
/// the row represents, and <see cref="Data"/> holds that entity's full payload verbatim, unevaluated.
/// </summary>
public class GcdRecord
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public int GcdId { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";
    public DateTime DateTimeIngested { get; set; } = DateTime.UtcNow;
}

namespace CC_Demo.Models.Marvel;

/// <summary>
/// A raw row from the <c>cc_demo</c> table, as ingested from the Marvel API.
/// Unlike <see cref="Creator"/>, this is not normalized -
/// the <see cref="Resource"/> column distinguishes which Marvel resource type
/// (creators, characters, series, stories, comics, events) the row represents,
/// and <see cref="Data"/> holds that resource's full JSON payload verbatim.
/// </summary>
public class MarvelRecord
{
    public Ulid Id { get; set; }
    public int MarvelId { get; set; }
    public string AttributionHtml { get; set; } = string.Empty;
    public string AttributionText { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";
    public string Resource { get; set; } = string.Empty;
    public string ResourceUri { get; set; } = string.Empty;
}

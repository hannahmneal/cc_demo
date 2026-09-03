using System.Text.Json.Serialization;
using CC_Demo.Models.Marvel;

namespace CC_Demo.Models;

public class Creator
{
    [JsonPropertyName("id")]
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [JsonPropertyName("marvelId")]
    public int MarvelId { get; set; }
    
    // TODO: Creator Type
    
    // TODO: Creator References (urls, links to portfolios, socials, etc.; use LLM to discover)

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("suffix")]
    public string Suffix { get; set; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    // TODO: Ensure this is distinct from Marvel's `modified` attr. Create two `modified` dates to distinguish.
    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; } // Marvel API returns a sentinel out-of-range date when a creator has never been modified.

    // TODO: "Required Attributions" class?
    [JsonPropertyName("attributionHtml")]
    public required string AttributionHtml { get; set; }

    // TODO: "Required Attributions" class?
    [JsonPropertyName("attributionText")]
    public required string AttributionText { get; set; }

    // TODO: "Required Attributions" class?
    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = string.Empty;

    // When this row was ingested into our own database - distinct from DateTimeCreated (Marvel's own
    // creation concept). Not served to API consumers; internal bookkeeping only.
    [JsonPropertyName("datetimeIngested")]
    [JsonIgnore]
    public DateTime DateTimeIngested { get; set; } = DateTime.UtcNow; // Set once upon initialization; cannot be changed once set.

    // TODO: change spelling to `datetimeCreated`
    [JsonPropertyName("dateTimeCreated")]
    public DateTime DateTimeCreated { get; set; } = DateTime.UtcNow; // Set once upon initialization; cannot be changed once set.

    // TODO: If we have a `Creators` table, is this attr necessary?
    [JsonPropertyName("resource")]
    public string Resource { get; set; } = "creators";

    // TODO: If MDA's resourceUri is non-functional, remove this attr 
    [JsonPropertyName("resourceURI")]
    public string ResourceUri { get; set; } = string.Empty;

    [JsonPropertyName("thumbnail")]
    public ThumbnailMarvel Thumbnail { get; set; } = new();

    [JsonPropertyName("urls")]
    public List<UrlMarvel> Urls { get; set; } = [];
    
    public int Version { get; set; } = 1; // TODO: 1 upon initialization, n + 1 after each update

    [JsonPropertyName("comics")]
    public ComicsMarvel Comics { get; set; } = new();

    [JsonPropertyName("events")]
    public EventsMarvel Events { get; set; } = new();

    [JsonPropertyName("series")]
    public SeriesMarvel Series { get; set; } = new();

    [JsonPropertyName("stories")]
    public StoriesMarvel Stories { get; set; } = new();
}

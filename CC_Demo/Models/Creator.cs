using System.Text.Json.Serialization;
using CC_Demo.Models.Marvel;

namespace CC_Demo.Data;

public class Creator
{
    [JsonPropertyName("id")]
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [JsonPropertyName("marvelId")]
    public int MarvelId { get; set; }

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

    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; } // Marvel API returns a sentinel out-of-range date when a creator has never been modified.

    [JsonPropertyName("thumbnail")]
    public ThumbnailMarvel Thumbnail { get; set; } = new();

    [JsonPropertyName("urls")]
    public List<UrlMarvel> Urls { get; set; } = [];

    [JsonPropertyName("attributionHtml")]
    public required string AttributionHtml { get; set; }

    [JsonPropertyName("attributionText")]
    public required string AttributionText { get; set; }

    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = string.Empty;

    [JsonPropertyName("datetimeAdded")]
    public DateTime DatetimeAdded { get; set; } = DateTime.UtcNow; // TODO: If no value, set one upon initialization; cannot be changed once set!

    [JsonPropertyName("dateTimeCreated")]
    public DateTime DateTimeCreated { get; set; } = DateTime.UtcNow; // Set once upon initialization; cannot be changed once set.

    [JsonPropertyName("resource")]
    public string Resource { get; set; } = "creators";

    [JsonPropertyName("resourceURI")]
    public string ResourceUri { get; set; } = string.Empty;

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

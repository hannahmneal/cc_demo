using System.Text.Json.Serialization;

namespace CC_Demo.Models.Marvel;

public class CreatorMarvel: CollectionMarvel<ItemMarvel>
{
    /// <summary>
    /// Marvel's unique identifier for the entity.
    /// </summary>
    [JsonPropertyName("id")]
    public int? id { get; set; }
    
    [JsonPropertyName("attributionHtml")]
    public required string AttributionHtml { get; set; }

    [JsonPropertyName("attributionText")]
    public required string AttributionText { get; set; }
    
    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = string.Empty;
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
    
    [JsonPropertyName("suffix")]
    public string Suffix { get; set; } = string.Empty;
    
    [JsonPropertyName("thumbnail")]
    public ThumbnailMarvel Thumbnail { get; set; } = new();
    
    [JsonPropertyName("urls")]
    public List<UrlMarvel> Urls { get; set; } = [];
    
    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; } // Marvel API returns a sentinel out-of-range date when a creator has never been modified.
    
    [JsonPropertyName("resource")]
    public string Resource { get; set; } = "creators";

    [JsonPropertyName("resourceURI")]
    public string ResourceUri { get; set; } = string.Empty;

    [JsonPropertyName("comics")]
    public ComicsMarvel Comics { get; set; } = new();
    
    [JsonPropertyName("events")]
    public EventsMarvel Events { get; set; } = new();
 
    [JsonPropertyName("series")]
    public SeriesMarvel Series { get; set; } = new();
    
    [JsonPropertyName("stories")]
    public StoriesMarvel Stories { get; set; } = new();
}

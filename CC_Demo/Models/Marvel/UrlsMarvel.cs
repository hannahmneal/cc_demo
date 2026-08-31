using System.Text.Json.Serialization;

namespace CC_Demo.Models.Marvel;

public class UrlMarvel
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class UrlsMarvel
{
    [JsonPropertyName("urls")]
    public List<UrlMarvel> Urls { get; set; } = new();
    
}


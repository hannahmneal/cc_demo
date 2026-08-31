using System.Text.Json.Serialization;

namespace CC_Demo.Models.Marvel;

public class ThumbnailMarvel
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("extension")]
    public string Extension { get; set; } = string.Empty;
}

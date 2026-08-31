using System.Text.Json.Serialization;

namespace CC_Demo.Models.Marvel;

public class ItemMarvel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("resourceURI")]
    public string ResourceUri { get; set; } = string.Empty;
}


public class ItemWithTypeMarvel: ItemMarvel
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
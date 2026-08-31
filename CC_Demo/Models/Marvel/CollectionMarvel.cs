using System.Text.Json.Serialization;

namespace CC_Demo.Models.Marvel;

public class CollectionMarvel<TItem>
{
    [JsonPropertyName("items")]
    public List<TItem> Items { get; set; } = [];

    [JsonPropertyName("returned")]
    public int Returned { get; set; } = 0;
    
    [JsonPropertyName("available")]
    public int Available { get; set; } = 0;
    
    [JsonPropertyName("collectionURI")]
    public string CollectionUri { get; set; } = string.Empty;
}
namespace CC_Demo.Models.Marvel;

public class CreatorMarvel: CollectionMarvel<ItemMarvel>
{
    public int? id { get; set; }

    public ComicsMarvel Comics { get; set; } = new();
    
    public EventsMarvel Events { get; set; } = new();
 
    public SeriesMarvel Series { get; set; } = new();
    
    public StoriesMarvel Stories { get; set; } = new();

    public List<UrlMarvel> Urls { get; set; } = [];

}
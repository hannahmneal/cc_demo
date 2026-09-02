namespace CC_Demo.Models.Marvel;

// TODO: Unused. Could be base class for Required Attributions?

public class MarvelRecord
{
    public int MarvelId { get; set; }
    public string AttributionHtml { get; set; } = string.Empty;
    public string AttributionText { get; set; } = string.Empty;
    public string CopyrightHtml { get; set; } = string.Empty;
    // data
    
    public DateTime OnSaleDate { get; set; } = default;
}



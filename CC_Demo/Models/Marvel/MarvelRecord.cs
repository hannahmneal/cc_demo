namespace CC_Demo.Data;

public class MarvelRecord
{
    public int MarvelId { get; set; }
    public string AttributionHtml { get; set; } = string.Empty;
    public string AttributionText { get; set; } = string.Empty;
    public string CopyrightHtml { get; set; } = string.Empty;
    // data
    public DateTime OnSaleDate { get; set; } = default;
}



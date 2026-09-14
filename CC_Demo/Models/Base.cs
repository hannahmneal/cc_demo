using Cysharp.Serialization.Json;

namespace CC_Demo.Data;

public class Base
{
    // TODO: You will need to transform from DB's 🐍 case to 🐪 case
    
    // The id is stored as a varchar in the database,
    // but it should be enforced as a ULID in code
    public string id { get; set; }
    public int marvelId { get; set; }
    public string attributionHtml { get; set; }
    public string attributionText { get; set; }
    public string copyrightHtml { get; set; }
    // data
    public DateTime datetimeIngested { get; set; }
    public string resource { get; set; }
    public string resourceUrl { get; set; }
    public int version { get; set; }
}
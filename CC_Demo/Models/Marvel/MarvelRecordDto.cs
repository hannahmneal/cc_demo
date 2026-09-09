using System.Text.Json;

namespace CC_Demo.Models.Marvel;

/// <summary>
/// A JSON representation of a raw <see cref="MarvelRecord"/>, with <see cref="Data"/> exposed as
/// nested JSON rather than an escaped string.
/// </summary>
public record MarvelRecordDto(
    Ulid Id,
    int MarvelId,
    string AttributionHtml,
    string AttributionText,
    string Copyright,
    JsonElement Data,
    string Resource,
    string ResourceUri)
{
    public static MarvelRecordDto FromEntity(MarvelRecord record) => new(
        record.Id,
        record.MarvelId,
        record.AttributionHtml,
        record.AttributionText,
        record.Copyright,
        JsonSerializer.Deserialize<JsonElement>(record.Data),
        record.Resource,
        record.ResourceUri);
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CC_Demo.Models.Pagination;

/// <summary>
/// Serializes any <see cref="PagedResult{T}"/> with its items array keyed by
/// <see cref="PagedResult{T}.ItemsName"/> - e.g. for Creators:
/// <c>{ "creators": [...], "totalCount": 952, ... }</c>.
/// One converter handles every entity type; no per-entity subclass of <see cref="PagedResult{T}"/> is needed.
/// </summary>
public class PagedResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(PagedResult<>);

    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(PagedResultJsonConverter<>).MakeGenericType(itemType);
        return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private class PagedResultJsonConverter<T> : JsonConverter<PagedResult<T>>
    {
        public override PagedResult<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotSupportedException($"Deserializing {nameof(PagedResult<T>)} is not supported.");

        public override void Write(Utf8JsonWriter writer, PagedResult<T> value, JsonSerializerOptions options)
        {
            string Name(string propertyName) => options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;

            writer.WriteStartObject();

            writer.WritePropertyName(value.ItemsName);
            JsonSerializer.Serialize(writer, value.Items, options);

            writer.WriteNumber(Name(nameof(PagedResult<T>.TotalCount)), value.TotalCount);
            writer.WriteNumber(Name(nameof(PagedResult<T>.Page)), value.Page);
            writer.WriteNumber(Name(nameof(PagedResult<T>.PageSize)), value.PageSize);
            writer.WriteNumber(Name(nameof(PagedResult<T>.TotalPages)), value.TotalPages);

            writer.WriteEndObject();
        }
    }
}

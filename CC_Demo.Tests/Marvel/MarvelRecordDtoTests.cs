using System.Text.Json;
using CC_Demo.Models.Marvel;

namespace CC_Demo.Tests.Marvel;

public class MarvelRecordDtoTests
{
    // The API serves controllers with ASP.NET Core's web defaults (camelCase).
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    private const string CreatorData = """
        {"id":13774,"fullName":"Danny Miki","comics":{"available":4,"items":[{"name":"X-Men"}]}}
        """;

    [Fact]
    public void FromEntity_copies_scalar_fields_verbatim()
    {
        var id = Ulid.NewUlid();
        var entity = new MarvelRecord
        {
            Id = id,
            MarvelId = 13774,
            AttributionHtml = "<a>Marvel</a>",
            AttributionText = "Marvel",
            Copyright = "© 2024 MARVEL",
            Data = CreatorData,
            Resource = "creators",
            ResourceUri = "http://gateway.marvel.com/v1/public/creators/13774",
        };

        var dto = MarvelRecordDto.FromEntity(entity);

        Assert.Equal(id, dto.Id);
        Assert.Equal(13774, dto.MarvelId);
        Assert.Equal("<a>Marvel</a>", dto.AttributionHtml);
        Assert.Equal("Marvel", dto.AttributionText);
        Assert.Equal("© 2024 MARVEL", dto.Copyright);
        Assert.Equal("creators", dto.Resource);
        Assert.Equal("http://gateway.marvel.com/v1/public/creators/13774", dto.ResourceUri);
    }

    [Fact]
    public void FromEntity_parses_the_data_string_into_a_navigable_json_element()
    {
        var dto = MarvelRecordDto.FromEntity(Record(CreatorData));

        Assert.Equal(JsonValueKind.Object, dto.Data.ValueKind);
        Assert.Equal(13774, dto.Data.GetProperty("id").GetInt32());
        Assert.Equal("Danny Miki", dto.Data.GetProperty("fullName").GetString());
        Assert.Equal(4, dto.Data.GetProperty("comics").GetProperty("available").GetInt32());
    }

    [Fact]
    public void Serialized_dto_exposes_data_as_nested_json_not_an_escaped_string()
    {
        var dto = MarvelRecordDto.FromEntity(Record(CreatorData));

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(dto, Web));
        var data = doc.RootElement.GetProperty("data");

        // The whole point of the DTO: `data` must be a JSON object, not a quoted string.
        Assert.Equal(JsonValueKind.Object, data.ValueKind);
        Assert.Equal("Danny Miki", data.GetProperty("fullName").GetString());
    }

    [Fact]
    public void Serialized_dto_renders_the_id_as_a_canonical_ulid_string()
    {
        var id = Ulid.NewUlid();

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(MarvelRecordDto.FromEntity(Record(CreatorData, id)), Web));

        Assert.Equal(id.ToString(), doc.RootElement.GetProperty("id").GetString());
    }

    [Fact]
    public void FromEntity_surfaces_malformed_data_as_a_json_exception()
    {
        // Characterization: `Data` is trusted to be valid JSON; a bad row fails loudly rather than silently.
        Assert.Throws<JsonException>(() => MarvelRecordDto.FromEntity(Record("{ not json")));
    }

    private static MarvelRecord Record(string data, Ulid? id = null) => new()
    {
        Id = id ?? Ulid.NewUlid(),
        MarvelId = 13774,
        AttributionHtml = "<a>Marvel</a>",
        AttributionText = "Marvel",
        Copyright = "© 2024 MARVEL",
        Data = data,
        Resource = "creators",
        ResourceUri = "http://gateway.marvel.com/v1/public/creators/13774",
    };
}

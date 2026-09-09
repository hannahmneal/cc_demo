using System.Text.Json;
using CC_Demo.Models;

namespace CC_Demo.Tests.Serialization;

/// <summary>
/// The API serves <see cref="Creator"/> straight to clients, so its JSON contract - property
/// names, ignored internals, nested shapes, nullable dates - is part of the public surface.
/// </summary>
public class CreatorSerializationTests
{
    private const string CreatorJson = """
    {
      "id": "01KKMFCCKHV9YDHTS50P4H5DBM",
      "marvelId": 13774,
      "firstName": "Danny",
      "lastName": "Miki",
      "fullName": "Danny Miki",
      "modified": null,
      "attributionHtml": "<a href=\"http://marvel.com\">Data provided by Marvel. © 2024 MARVEL</a>",
      "attributionText": "Data provided by Marvel. © 2024 MARVEL",
      "copyright": "© 2024 MARVEL",
      "thumbnail": { "path": "http://i.annihil.us/foo", "extension": "jpg" },
      "urls": [ { "url": "http://marvel.com/creators/13774", "type": "detail" } ],
      "comics": {
        "available": 4,
        "returned": 1,
        "items": [ { "name": "X-Men Milestones", "resourceURI": "http://gateway.marvel.com/v1/public/comics/82314" } ]
      }
    }
    """;

    [Fact]
    public void Deserializes_a_marvel_style_payload_including_nested_shapes()
    {
        var creator = JsonSerializer.Deserialize<Creator>(CreatorJson)!;

        Assert.Equal(Ulid.Parse("01KKMFCCKHV9YDHTS50P4H5DBM"), creator.Id);
        Assert.Equal(13774, creator.MarvelId);
        Assert.Equal("Danny Miki", creator.FullName);
        Assert.Null(creator.Modified);
        Assert.Equal("jpg", creator.Thumbnail.Extension);
        Assert.Single(creator.Urls);
        Assert.Equal("detail", creator.Urls[0].Type);
        Assert.Equal(4, creator.Comics.Available);
        Assert.Equal("X-Men Milestones", creator.Comics.Items.Single().Name);
    }

    [Fact]
    public void Round_trips_without_losing_data()
    {
        var once = JsonSerializer.Deserialize<Creator>(CreatorJson)!;
        var twice = JsonSerializer.Deserialize<Creator>(JsonSerializer.Serialize(once))!;

        Assert.Equal(once.Id, twice.Id);
        Assert.Equal(once.MarvelId, twice.MarvelId);
        Assert.Equal(once.FullName, twice.FullName);
        Assert.Equal(once.Comics.Items.Single().Name, twice.Comics.Items.Single().Name);
        Assert.Equal(once.Urls[0].Url, twice.Urls[0].Url);
    }

    [Fact]
    public void Uses_json_property_names_not_clr_names()
    {
        var creator = JsonSerializer.Deserialize<Creator>(CreatorJson)!;

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(creator));
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("marvelId", out _));
        Assert.True(root.TryGetProperty("attributionHtml", out _));
        Assert.True(root.TryGetProperty("resourceURI", out _));
        Assert.False(root.TryGetProperty("MarvelId", out _));
    }

    [Fact]
    public void Does_not_leak_the_internal_ingestion_timestamp()
    {
        var creator = JsonSerializer.Deserialize<Creator>(CreatorJson)!;

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(creator));

        // DateTimeIngested is [JsonIgnore] - internal bookkeeping, never served.
        Assert.False(doc.RootElement.TryGetProperty("datetimeIngested", out _));
        Assert.False(doc.RootElement.TryGetProperty("dateTimeIngested", out _));
    }

    [Fact]
    public void Id_serializes_as_its_canonical_string_form()
    {
        var creator = JsonSerializer.Deserialize<Creator>(CreatorJson)!;

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(creator));

        Assert.Equal("01KKMFCCKHV9YDHTS50P4H5DBM", doc.RootElement.GetProperty("id").GetString());
    }

    [Fact]
    public void Missing_required_attribution_fields_fail_deserialization()
    {
        var json = """{ "id": "01KKMFCCKHV9YDHTS50P4H5DBM", "marvelId": 1 }""";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Creator>(json));
    }
}

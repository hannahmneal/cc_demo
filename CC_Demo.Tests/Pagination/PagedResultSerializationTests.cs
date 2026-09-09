using System.Text.Json;
using CC_Demo.Models.Pagination;

namespace CC_Demo.Tests.Pagination;

public class PagedResultSerializationTests
{
    private static JsonSerializerOptions Options()
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        options.Converters.Add(new PagedResultJsonConverterFactory());
        return options;
    }

    [Fact]
    public void Items_are_written_under_the_ItemsName_key_alongside_camelCased_metadata()
    {
        var page = new PagedResult<string>(["a", "b"], TotalCount: 952, Page: 2, PageSize: 20, TotalPages: 48)
        {
            ItemsName = "creators",
        };

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(page, Options()));
        var root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.GetProperty("creators").ValueKind);
        Assert.Equal(2, root.GetProperty("creators").GetArrayLength());
        Assert.Equal(952, root.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, root.GetProperty("page").GetInt32());
        Assert.Equal(20, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(48, root.GetProperty("totalPages").GetInt32());
    }

    [Fact]
    public void ItemsName_itself_is_never_serialized_and_there_is_no_generic_items_key()
    {
        var page = new PagedResult<string>(["a"], 1, 1, 20, 1) { ItemsName = "creators" };

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(page, Options()));

        Assert.False(doc.RootElement.TryGetProperty("items", out _));
        Assert.False(doc.RootElement.TryGetProperty("itemsName", out _));
    }

    [Fact]
    public void ItemsName_defaults_to_items()
    {
        var page = new PagedResult<int>([1, 2, 3], 3, 1, 20, 1);

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(page, Options()));

        Assert.Equal(3, doc.RootElement.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public void Deserializing_a_PagedResult_is_not_supported()
    {
        var json = """{ "creators": [], "totalCount": 0, "page": 1, "pageSize": 20, "totalPages": 0 }""";

        Assert.Throws<NotSupportedException>(
            () => JsonSerializer.Deserialize<PagedResult<string>>(json, Options()));
    }

    [Fact]
    public void Select_projects_items_while_preserving_all_paging_metadata()
    {
        var source = new PagedResult<int>([1, 2, 3], TotalCount: 3, Page: 1, PageSize: 20, TotalPages: 1)
        {
            ItemsName = "numbers",
        };

        var mapped = source.Select(n => $"#{n}");

        Assert.Equal(["#1", "#2", "#3"], mapped.Items);
        Assert.Equal(3, mapped.TotalCount);
        Assert.Equal(1, mapped.Page);
        Assert.Equal(20, mapped.PageSize);
        Assert.Equal(1, mapped.TotalPages);
        Assert.Equal("numbers", mapped.ItemsName);
    }
}

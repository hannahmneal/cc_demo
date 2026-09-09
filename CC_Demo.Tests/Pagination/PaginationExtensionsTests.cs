using CC_Demo.Models;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using CC_Demo.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Tests.Pagination;

public class PaginationExtensionsTests : IDisposable
{
    private readonly SqliteFixture _fx = new();

    public void Dispose() => _fx.Dispose();

    private async Task SeedCreators(int count)
    {
        for (var i = 1; i <= count; i++)
            _fx.Db.Creator.Add(SqliteFixture.NewCreator(marvelId: i, fullName: $"Creator {i:D3}"));
        await _fx.Db.SaveChangesAsync();
    }

    private IQueryable<Creator> OrderedQuery() =>
        _fx.NewContext().Creator.AsNoTracking().OrderBy(c => c.MarvelId);

    [Fact]
    public async Task First_page_returns_page_size_items_and_correct_metadata()
    {
        await SeedCreators(45);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(1, 20));

        Assert.Equal(20, result.Items.Count);
        Assert.Equal(45, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(1, result.Items.First().MarvelId);
        Assert.Equal(20, result.Items.Last().MarvelId);
    }

    [Fact]
    public async Task Middle_page_skips_the_right_number_of_rows()
    {
        await SeedCreators(45);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(2, 20));

        Assert.Equal(21, result.Items.First().MarvelId);
        Assert.Equal(40, result.Items.Last().MarvelId);
    }

    [Fact]
    public async Task Final_page_returns_only_the_remainder()
    {
        await SeedCreators(45);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(3, 20));

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(41, result.Items.First().MarvelId);
        Assert.Equal(45, result.Items.Last().MarvelId);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task Page_past_the_end_is_empty_but_still_reports_totals()
    {
        await SeedCreators(45);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(99, 20));

        Assert.Empty(result.Items);
        Assert.Equal(45, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(99, result.Page);
    }

    [Fact]
    public async Task Null_page_size_returns_every_row_on_a_single_page()
    {
        await SeedCreators(45);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(1, null));

        Assert.Equal(45, result.Items.Count);
        Assert.Equal(45, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(45, result.PageSize);   // PageSize echoes the full count for "all"
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task Empty_table_reports_zero_pages_for_a_sized_request()
    {
        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(1, 20));

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task Empty_table_reports_zero_pages_for_an_all_request()
    {
        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(1, null));

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task Exact_multiple_of_page_size_does_not_add_a_trailing_page()
    {
        await SeedCreators(40);

        var result = await OrderedQuery().ToPagedResultAsync(new PaginationRequest(1, 20));

        Assert.Equal(2, result.TotalPages);
    }
}

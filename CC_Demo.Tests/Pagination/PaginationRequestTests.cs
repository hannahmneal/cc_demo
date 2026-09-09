using CC_Demo.Models.Pagination;

namespace CC_Demo.Tests.Pagination;

public class PaginationRequestTests
{
    [Theory]
    [InlineData("20", 20)]
    [InlineData("50", 50)]
    [InlineData("100", 100)]
    public void TryParse_accepts_each_allowed_page_size(string pageSize, int expected)
    {
        var ok = PaginationRequest.TryParse(page: 3, pageSize, out var request, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(3, request.Page);
        Assert.Equal(expected, request.PageSize);
    }

    [Theory]
    [InlineData("all")]
    [InlineData("ALL")]
    [InlineData("All")]
    public void TryParse_treats_all_case_insensitively_as_no_limit(string pageSize)
    {
        var ok = PaginationRequest.TryParse(page: 5, pageSize, out var request, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Null(request.PageSize);
        // "all" collapses to a single page regardless of the page the caller asked for.
        Assert.Equal(1, request.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void TryParse_rejects_page_below_one(int page)
    {
        var ok = PaginationRequest.TryParse(page, "20", out _, out var error);

        Assert.False(ok);
        Assert.Equal("'page' must be 1 or greater.", error);
    }

    [Fact]
    public void TryParse_validates_page_before_page_size_even_for_all()
    {
        var ok = PaginationRequest.TryParse(page: 0, "all", out _, out var error);

        Assert.False(ok);
        Assert.Equal("'page' must be 1 or greater.", error);
    }

    [Theory]
    [InlineData("7")]
    [InlineData("0")]
    [InlineData("21")]
    [InlineData("1000000")]
    [InlineData("-20")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("20.0")]
    public void TryParse_rejects_disallowed_page_size(string pageSize)
    {
        var ok = PaginationRequest.TryParse(page: 1, pageSize, out _, out var error);

        Assert.False(ok);
        Assert.Equal("'pageSize' must be one of 20, 50, 100, or 'all'.", error);
    }

    [Fact]
    public void AllowedPageSizes_is_the_documented_set()
    {
        Assert.Equal(new[] { 20, 50, 100 }, PaginationRequest.AllowedPageSizes);
    }

    [Fact]
    public void Records_with_the_same_values_are_equal()
    {
        Assert.Equal(new PaginationRequest(2, 50), new PaginationRequest(2, 50));
        Assert.NotEqual(new PaginationRequest(2, 50), new PaginationRequest(2, 20));
    }
}

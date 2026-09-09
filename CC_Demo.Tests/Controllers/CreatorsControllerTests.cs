using CC_Demo.Controllers;
using CC_Demo.Models;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using CC_Demo.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace CC_Demo.Tests.Controllers;

public class CreatorsControllerTests : IDisposable
{
    private readonly SqliteFixture _fx = new();
    private readonly CreatorsController _controller;

    public CreatorsControllerTests() => _controller = new CreatorsController(new CreatorRepository(_fx.Db));

    public void Dispose() => _fx.Dispose();

    private async Task<Creator> SeedOne(int marvelId = 3788, string name = "Jack Kirby")
    {
        var creator = SqliteFixture.NewCreator(marvelId, name);
        _fx.Db.Creator.Add(creator);
        await _fx.Db.SaveChangesAsync();
        return creator;
    }

    // ---- GetById -------------------------------------------------------------

    [Fact]
    public async Task GetById_with_a_ulid_returns_the_creator()
    {
        var creator = await SeedOne();

        var result = await _controller.GetById(creator.Id.ToString(), default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(creator.Id, Assert.IsType<Creator>(ok.Value).Id);
    }

    [Fact]
    public async Task GetById_with_an_integer_resolves_by_marvel_id()
    {
        await SeedOne(marvelId: 3788, name: "Jack Kirby");

        var result = await _controller.GetById("3788", default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(3788, Assert.IsType<Creator>(ok.Value).MarvelId);
    }

    [Fact]
    public async Task GetById_with_an_unknown_ulid_is_404()
    {
        await SeedOne();

        var result = await _controller.GetById(Ulid.NewUlid().ToString(), default);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_with_an_unknown_integer_is_404()
    {
        await SeedOne(marvelId: 3788);

        var result = await _controller.GetById("999999", default);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData("not-a-ulid")]
    [InlineData("01KKMFCCKHV9YDHTS50P4H5DB")]   // 25 chars - too short for a ULID
    [InlineData("")]
    public async Task GetById_with_a_value_that_is_neither_ulid_nor_int_is_400(string id)
    {
        var result = await _controller.GetById(id, default);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("not a valid Ulid or integer", bad.Value!.ToString());
    }

    // ---- GetAll -------------------------------------------------------------

    [Fact]
    public async Task GetAll_returns_a_page_labelled_creators()
    {
        for (var i = 1; i <= 5; i++) await SeedOne(i, $"Creator {i}");

        var result = await _controller.GetAll(page: 1, pageSize: "20");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<Creator>>(ok.Value);
        Assert.Equal("creators", page.ItemsName);
        Assert.Equal(5, page.TotalCount);
    }

    [Theory]
    [InlineData(0, "20")]
    [InlineData(1, "7")]
    [InlineData(1, "abc")]
    [InlineData(-3, "all")]
    public async Task GetAll_with_bad_pagination_params_is_400(int page, string pageSize)
    {
        var result = await _controller.GetAll(page, pageSize);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_with_all_returns_every_creator_on_one_page()
    {
        for (var i = 1; i <= 25; i++) await SeedOne(i, $"Creator {i}");

        var result = await _controller.GetAll(page: 1, pageSize: "all");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<Creator>>(ok.Value);
        Assert.Equal(25, page.Items.Count);
        Assert.Equal(1, page.TotalPages);
    }
}

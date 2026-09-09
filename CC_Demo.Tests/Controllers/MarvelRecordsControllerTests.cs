using System.Text.Json;
using CC_Demo.Controllers;
using CC_Demo.Models.Marvel;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using CC_Demo.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace CC_Demo.Tests.Controllers;

public class MarvelRecordsControllerTests : IDisposable
{
    private readonly SqliteFixture _fx = new();
    private readonly MarvelRecordsController _controller;

    public MarvelRecordsControllerTests() =>
        _controller = new MarvelRecordsController(new MarvelRecordRepository(_fx.Db));

    public void Dispose() => _fx.Dispose();

    private async Task<MarvelRecord> Seed(string resource, int marvelId, string data = "{}")
    {
        var record = SqliteFixture.NewRecord(resource, marvelId, data);
        _fx.Db.MarvelRecords.Add(record);
        await _fx.Db.SaveChangesAsync();
        return record;
    }

    // ---- resource validation ---------------------------------------------------

    [Theory]
    [InlineData("widgets")]
    [InlineData("creator")]
    public async Task Unknown_resource_type_is_404_on_both_endpoints(string resource)
    {
        var all = await _controller.GetAll(resource, page: 1, pageSize: "20");
        Assert.IsType<NotFoundObjectResult>(all.Result);

        var byId = await _controller.GetById(resource, "1", default);
        Assert.IsType<NotFoundObjectResult>(byId.Result);
    }

    // ---- GetById -------------------------------------------------------------

    [Fact]
    public async Task GetById_with_an_integer_resolves_by_marvel_id()
    {
        await Seed("creators", 3788);

        var result = await _controller.GetById("creators", "3788", default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(3788, Assert.IsType<MarvelRecordDto>(ok.Value).MarvelId);
    }

    [Fact]
    public async Task GetById_with_a_ulid_resolves_by_primary_key()
    {
        var record = await Seed("creators", 3788);

        var result = await _controller.GetById("creators", record.Id.ToString(), default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(record.Id, Assert.IsType<MarvelRecordDto>(ok.Value).Id);
    }

    [Fact]
    public async Task GetById_does_not_return_a_record_from_another_resource_bucket()
    {
        await Seed("characters", 3788);

        var byInt = await _controller.GetById("creators", "3788", default);
        Assert.IsType<NotFoundResult>(byInt.Result);
    }

    [Fact]
    public async Task GetById_with_an_unknown_integer_is_404()
    {
        await Seed("creators", 3788);

        var result = await _controller.GetById("creators", "999999", default);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData("not-a-ulid")]
    [InlineData("3788x")]
    public async Task GetById_with_a_non_ulid_non_integer_id_is_400(string id)
    {
        var result = await _controller.GetById("creators", id, default);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("not a valid Ulid or integer", bad.Value!.ToString());
    }

    [Fact]
    public async Task GetById_exposes_data_as_nested_json()
    {
        await Seed("creators", 3788, data: """{"fullName":"Jack Kirby","comics":{"available":1122}}""");

        var result = await _controller.GetById("creators", "3788", default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<MarvelRecordDto>(ok.Value);
        Assert.Equal(JsonValueKind.Object, dto.Data.ValueKind);
        Assert.Equal("Jack Kirby", dto.Data.GetProperty("fullName").GetString());
    }

    // ---- GetAll -------------------------------------------------------------

    [Fact]
    public async Task GetAll_returns_only_the_requested_resource_labelled_by_resource_name()
    {
        await Seed("creators", 1);
        await Seed("creators", 2);
        await Seed("characters", 3);

        var result = await _controller.GetAll("creators", page: 1, pageSize: "20");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<MarvelRecordDto>>(ok.Value);
        Assert.Equal("creators", page.ItemsName);
        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, dto => Assert.Equal("creators", dto.Resource));
    }

    [Theory]
    [InlineData(0, "20")]
    [InlineData(1, "7")]
    [InlineData(1, "nonsense")]
    public async Task GetAll_with_bad_pagination_params_is_400(int page, string pageSize)
    {
        var result = await _controller.GetAll("creators", page, pageSize);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}

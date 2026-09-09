using CC_Demo.Models.Marvel;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using CC_Demo.Tests.TestSupport;

namespace CC_Demo.Tests;

public class MarvelRecordRepositoryTests : IDisposable
{
    private readonly SqliteFixture _fx = new();
    private readonly MarvelRecordRepository _repo;

    public MarvelRecordRepositoryTests() => _repo = new MarvelRecordRepository(_fx.Db);

    public void Dispose() => _fx.Dispose();

    private async Task Seed(params MarvelRecord[] records)
    {
        _fx.Db.MarvelRecords.AddRange(records);
        await _fx.Db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByIdAsync_matches_on_both_id_and_resource()
    {
        var creator = SqliteFixture.NewRecord("creators", 3788);
        await Seed(creator, SqliteFixture.NewRecord("characters", 3788));

        var found = await _repo.GetByIdAsync("creators", creator.Id);

        Assert.NotNull(found);
        Assert.Equal("creators", found!.Resource);
        Assert.Equal(3788, found.MarvelId);
    }

    [Fact]
    public async Task GetByIdAsync_does_not_cross_resource_boundaries()
    {
        var character = SqliteFixture.NewRecord("characters", 3788);
        await Seed(character);

        // Right ULID, wrong resource bucket.
        Assert.Null(await _repo.GetByIdAsync("creators", character.Id));
    }

    [Fact]
    public async Task GetByMarvelIdAsync_is_also_scoped_by_resource()
    {
        await Seed(
            SqliteFixture.NewRecord("creators", 3788),
            SqliteFixture.NewRecord("characters", 3788));

        var found = await _repo.GetByMarvelIdAsync("creators", 3788);

        Assert.NotNull(found);
        Assert.Equal("creators", found!.Resource);

        Assert.Null(await _repo.GetByMarvelIdAsync("events", 3788));
    }

    [Fact]
    public async Task GetAllAsync_returns_only_the_requested_resource_type()
    {
        await Seed(
            SqliteFixture.NewRecord("creators", 1),
            SqliteFixture.NewRecord("creators", 2),
            SqliteFixture.NewRecord("characters", 3),
            SqliteFixture.NewRecord("comics", 4));

        var result = await _repo.GetAllAsync("creators", new PaginationRequest(1, 20));

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, r => Assert.Equal("creators", r.Resource));
    }

    [Fact]
    public async Task GetAllAsync_paginates_within_a_resource_type()
    {
        for (var i = 1; i <= 25; i++)
            _fx.Db.MarvelRecords.Add(SqliteFixture.NewRecord("creators", i));
        await _fx.Db.SaveChangesAsync();

        var page1 = await _repo.GetAllAsync("creators", new PaginationRequest(1, 20));
        var page2 = await _repo.GetAllAsync("creators", new PaginationRequest(2, 20));

        Assert.Equal(20, page1.Items.Count);
        Assert.Equal(5, page2.Items.Count);
        Assert.Equal(25, page1.TotalCount);
        Assert.Equal(2, page1.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_with_all_returns_every_row_of_the_resource()
    {
        for (var i = 1; i <= 25; i++)
            _fx.Db.MarvelRecords.Add(SqliteFixture.NewRecord("creators", i));
        await _fx.Db.SaveChangesAsync();

        var result = await _repo.GetAllAsync("creators", new PaginationRequest(1, null));

        Assert.Equal(25, result.Items.Count);
        Assert.Equal(1, result.TotalPages);
    }
}

using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using CC_Demo.Tests.TestSupport;

namespace CC_Demo.Tests;

public class CreatorRepositoryTests : IDisposable
{
    private readonly SqliteFixture _fx = new();
    private readonly CreatorRepository _repo;

    public CreatorRepositoryTests() => _repo = new CreatorRepository(_fx.Db);

    public void Dispose() => _fx.Dispose();

    private async Task Seed(params (int marvelId, string name)[] creators)
    {
        foreach (var (marvelId, name) in creators)
            _fx.Db.Creator.Add(SqliteFixture.NewCreator(marvelId, name));
        await _fx.Db.SaveChangesAsync();
    }

    [Fact]
    public async Task AddAsync_persists_a_creator_and_keeps_its_ulid_key()
    {
        var creator = SqliteFixture.NewCreator(13774, "Danny Miki");

        await _repo.AddAsync(creator);

        var reloaded = await new CreatorRepository(_fx.NewContext()).GetByIdAsync(creator.Id);
        Assert.NotNull(reloaded);
        Assert.Equal(creator.Id, reloaded!.Id);
        Assert.Equal("Danny Miki", reloaded.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_returns_the_match()
    {
        var creator = SqliteFixture.NewCreator(3788, "Jack Kirby");
        await _repo.AddAsync(creator);

        var found = await _repo.GetByIdAsync(creator.Id);

        Assert.NotNull(found);
        Assert.Equal(3788, found!.MarvelId);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_no_row_matches()
    {
        await Seed((1, "A"), (2, "B"));

        Assert.Null(await _repo.GetByIdAsync(Ulid.NewUlid()));
    }

    [Fact]
    public async Task GetByMarvelIdAsync_returns_the_match()
    {
        await Seed((13774, "Danny Miki"), (3788, "Jack Kirby"));

        var found = await _repo.GetByMarvelIdAsync(3788);

        Assert.NotNull(found);
        Assert.Equal("Jack Kirby", found!.FullName);
    }

    [Fact]
    public async Task GetByMarvelIdAsync_returns_null_when_no_row_matches()
    {
        await Seed((13774, "Danny Miki"));

        Assert.Null(await _repo.GetByMarvelIdAsync(999999));
    }

    [Fact]
    public async Task GetAllAsync_pages_and_orders_by_id()
    {
        for (var i = 1; i <= 30; i++)
            _fx.Db.Creator.Add(SqliteFixture.NewCreator(i, $"Creator {i:D3}"));
        await _fx.Db.SaveChangesAsync();

        var page2 = await _repo.GetAllAsync(new PaginationRequest(2, 20));

        Assert.Equal(10, page2.Items.Count);
        Assert.Equal(30, page2.TotalCount);
        Assert.Equal(2, page2.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_on_empty_table_returns_an_empty_page()
    {
        var result = await _repo.GetAllAsync(new PaginationRequest(1, 20));

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}

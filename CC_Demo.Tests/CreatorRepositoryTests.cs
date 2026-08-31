using CC_Demo.Data;
using CC_Demo.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Tests;

public class CreatorRepositoryTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    // [Fact]
    // public async Task AddAsync_persists_creator_and_assigns_id()
    // {
    //     var repository = new CreatorRepository(_db);
    //
    //     await repository.AddAsync(new Creator { AttributionHtml = "", AttributionText = "" });
    //
    //     var creators = await repository.GetAllAsync();
    //     Assert.Single(creators);
    //     Assert.Equal("", creators[0].AttributionHtml);
    //     Assert.True(creators[0].Id == Ulid);
    // }

    // [Fact]
    // public async Task GetByIdAsync_returns_null_when_not_found()
    // {
    //     var repository = new CreatorRepository(_db);
    //
    //     var result = await repository.GetByIdAsync(999);
    //
    //     Assert.Null(result);
    // }
}

using CC_Demo.Models.Marvel;
using CC_Demo.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var pgConnectionString = builder.Configuration["PG_CONNECTION"]
    ?? throw new InvalidOperationException("PG_CONNECTION environment variable is not set.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(pgConnectionString, npgsql => npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<ICreatorRepository, CreatorRepository>();

using var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var repository = scope.ServiceProvider.GetRequiredService<ICreatorRepository>();

    var creators = await repository.GetAllAsync();
    foreach (var creator in creators)
    {
        Console.WriteLine($"Creator {creator.Id}");
        
        Console.WriteLine(new ComicsMarvel());
        Console.WriteLine(new CreatorMarvel());
        Console.WriteLine(new EventsMarvel());
        Console.WriteLine(new SeriesMarvel());
        Console.WriteLine(new StoriesMarvel());
    }
}

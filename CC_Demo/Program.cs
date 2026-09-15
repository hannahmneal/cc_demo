using System.Reflection;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var pgConnectionString = builder.Configuration["PG_CONNECTION"]
    ?? throw new InvalidOperationException("PG_CONNECTION environment variable is not set.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(pgConnectionString, npgsql => npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<ICreatorRepository, CreatorRepository>();
builder.Services.AddScoped<IMarvelRecordRepository, MarvelRecordRepository>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new PagedResultJsonConverterFactory()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.MapControllers();

app.Run();

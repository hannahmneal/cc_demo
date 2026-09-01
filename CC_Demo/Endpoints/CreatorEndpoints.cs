using CC_Demo.Repository;

namespace CC_Demo.Endpoints;

public static class CreatorEndpoints
{
    public static void MapCreatorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/creators").WithTags("Creators");

        group.MapGet("/", async (ICreatorRepository repository, CancellationToken ct) =>
            Results.Ok(await repository.GetAllAsync(ct)));

        group.MapGet("/{id}", async (Ulid id, ICreatorRepository repository, CancellationToken ct) =>
        {
            var creator = await repository.GetByIdAsync(id, ct);
            return creator is null ? Results.NotFound() : Results.Ok(creator);
        });
    }
}

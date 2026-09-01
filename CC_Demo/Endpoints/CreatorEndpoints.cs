using CC_Demo.Repository;

namespace CC_Demo.Endpoints;

public static class CreatorEndpoints
{
    public static void MapCreatorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/creators").WithTags("Creators");

        group.MapGet("/", async (ICreatorRepository repository, CancellationToken ct) =>
            Results.Ok(await repository.GetAllAsync(ct)));

        group.MapGet("/{id}", async (string id, ICreatorRepository repository, CancellationToken ct) =>
        {
            if (int.TryParse(id, out var marvelId))
            {
                var byMarvelId = await repository.GetByMarvelIdAsync(marvelId, ct);
                return byMarvelId is null ? Results.NotFound() : Results.Ok(byMarvelId);
            }

            if (Ulid.TryParse(id, out var ulid))
            {
                var byId = await repository.GetByIdAsync(ulid, ct);
                return byId is null ? Results.NotFound() : Results.Ok(byId);
            }

            return Results.BadRequest($"'{id}' is not a valid Ulid or integer id.");
        });
    }
}

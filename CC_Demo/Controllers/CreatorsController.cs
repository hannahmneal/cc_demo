using CC_Demo.Data;
using CC_Demo.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CC_Demo.Controllers;

/// <summary>
/// Provides read access to <see cref="Creator"/> records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Creators")]
public class CreatorsController(ICreatorRepository repository) : ControllerBase
{
    /// <summary>
    /// Gets a list of <see cref="Creator"/> objects.
    /// </summary>
    /// <remarks>
    /// Returns every creator currently stored in the database, including each creator's
    /// nested Comics, Events, Series, and Stories collections.
    /// </remarks>
    /// <param name="ct">Cancellation token for the request.</param>
    /// <returns>A list of <see cref="Creator"/> objects.</returns>
    /// <response code="200">The list of creators was retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Creator>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Creator>>> GetAll(CancellationToken ct)
    {
        return Ok(await repository.GetAllAsync(ct));
    }

    /// <summary>
    /// Gets a single <see cref="Creator"/> by id.
    /// </summary>
    /// <remarks>
    /// The lookup mode is inferred from the format of <paramref name="id"/>.
    /// <para>A ULID (e.g. <c>01KKMFCCKHV9YDHTS50P4H5DBM</c>) matches the entity's primary key.</para>
    /// <para>An integer (e.g. <c>3788</c>) matches the entity's <see cref="Creator.MarvelId"/>.</para>
    /// <para>Sample requests: <c>GET /api/creators/01KKMFCCKHV9YDHTS50P4H5DBM</c> or <c>GET /api/creators/3788</c>.</para>
    /// </remarks>
    /// <param name="id">A ULID matching the entity's primary key, or an integer matching its MarvelId.</param>
    /// <param name="ct">Cancellation token for the request.</param>
    /// <returns>The matching <see cref="Creator"/>.</returns>
    /// <response code="200">A creator matching the given id was found.</response>
    /// <response code="400"><paramref name="id"/> was neither a valid ULID nor a valid integer.</response>
    /// <response code="404">No creator matches the given id.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Creator), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Creator>> GetById(string id, CancellationToken ct)
    {
        if (int.TryParse(id, out var marvelId))
        {
            var byMarvelId = await repository.GetByMarvelIdAsync(marvelId, ct);
            return byMarvelId is null ? NotFound() : Ok(byMarvelId);
        }

        if (Ulid.TryParse(id, out var ulid))
        {
            var byId = await repository.GetByIdAsync(ulid, ct);
            return byId is null ? NotFound() : Ok(byId);
        }

        return BadRequest($"'{id}' is not a valid Ulid or integer id.");
    }
}

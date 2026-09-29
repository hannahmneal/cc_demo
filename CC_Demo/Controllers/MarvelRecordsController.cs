using CC_Demo.Models.Marvel;
using CC_Demo.Models.Pagination;
using CC_Demo.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CC_Demo.Controllers;

/// <summary>
/// Provides read-only access to raw Marvel-sourced records (the <c>cc_demo</c> table) as ingested
/// from the Marvel Developer API, scoped by resource and by resource type (characters, comics,
/// creators, events, series, stories).
/// </summary>
[ApiController]
[Route("api/marvel/{resource}")]
[Tags("MarvelRecords")]
[ApiExplorerSettings(IgnoreApi = true)]
public class MarvelRecordsController(IMarvelRecordRepository repository) : ControllerBase
{
    /// <summary>
    /// Gets a page of raw records for the given Marvel resource type.
    /// </summary>
    /// <param name="resource">One of: creators, characters, series, stories, comics, events.</param>
    /// <param name="page">1-based page number. Defaults to 1.</param>
    /// <param name="pageSize">One of 20, 50, 100, or "all". Defaults to 20.</param>
    /// <param name="ct">Cancellation token for the request.</param>
    /// <returns>A page of raw records for the resource type, plus paging metadata.</returns>
    /// <response code="200">The page of records was retrieved successfully.</response>
    /// <response code="400"><paramref name="page"/> or <paramref name="pageSize"/> was invalid.</response>
    /// <response code="404"><paramref name="resource"/> is not a recognized Marvel resource type.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MarvelRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<MarvelRecordDto>>> GetAll(
        string resource, [FromQuery] int page = 1, [FromQuery] string pageSize = "20", CancellationToken ct = default)
    {
        if (!MarvelResourceTypes.IsValid(resource))
            return NotFound($"'{resource}' is not a recognized Marvel resource type.");

        if (!PaginationRequest.TryParse(page, pageSize, out var pagination, out var error))
            return BadRequest(error);

        var records = await repository.GetAllAsync(resource, pagination, ct);
        var result = records.Select(MarvelRecordDto.FromEntity) with { ItemsName = resource.ToLowerInvariant() };
        return Ok(result);
    }

    /// <summary>
    /// Gets a single raw record by id for the given Marvel resource type.
    /// </summary>
    /// <param name="resource">One of: creators, characters, series, stories, comics, events.</param>
    /// <param name="id">A ULID matching the record's primary key, or an integer matching its MarvelId.</param>
    /// <param name="ct">Cancellation token for the request.</param>
    /// <returns>The matching raw record.</returns>
    /// <response code="200">A record matching the given id was found.</response>
    /// <response code="400"><paramref name="id"/> was neither a valid ULID nor a valid integer.</response>
    /// <response code="404"><paramref name="resource"/> is not a recognized Marvel resource type, or no record matches the given id.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MarvelRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarvelRecordDto>> GetById(string resource, string id, CancellationToken ct)
    {
        if (!MarvelResourceTypes.IsValid(resource))
            return NotFound($"'{resource}' is not a recognized Marvel resource type.");

        if (int.TryParse(id, out var marvelId))
        {
            var byMarvelId = await repository.GetByMarvelIdAsync(resource, marvelId, ct);
            return byMarvelId is null ? NotFound() : Ok(MarvelRecordDto.FromEntity(byMarvelId));
        }

        if (!Ulid.TryParse(id, out var ulid))
            return BadRequest($"'{id}' is not a valid Ulid or integer id.");

        var record = await repository.GetByIdAsync(resource, ulid, ct);
        return record is null ? NotFound() : Ok(MarvelRecordDto.FromEntity(record));
    }
}

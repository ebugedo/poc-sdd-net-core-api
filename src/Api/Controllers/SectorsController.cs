using MediatR;
using Microsoft.AspNetCore.Mvc;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Application.Sectors.Queries.GetAllSectors;
using poc_sdd_net_core_api.Application.Sectors.Queries.GetSectorById;

namespace poc_sdd_net_core_api.Api.Controllers;

/// <summary>
/// Catalogo de sectores de solo lectura (BR-016, ADR-005).
/// No expone endpoints de escritura: el catalogo se siembra con la migracion.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class SectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SectorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var sectors = await _mediator.Send(new GetAllSectorsQuery());
        return Ok(sectors);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SectorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var sector = await _mediator.Send(new GetSectorByIdQuery(id));
            return Ok(sector);
        }
        catch (SectorNotFoundException)
        {
            return NotFound();
        }
    }
}

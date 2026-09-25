using MediatR;
using Microsoft.AspNetCore.Mvc;
using poc_sdd_net_core_api.Application.Clients.Commands.CreateClient;
using poc_sdd_net_core_api.Application.Clients.Commands.DeleteClient;
using poc_sdd_net_core_api.Application.Clients.Commands.UpdateClient;
using poc_sdd_net_core_api.Application.Clients.Queries.GetAllClients;
using poc_sdd_net_core_api.Application.Clients.Queries.GetClientById;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _mediator.Send(new GetAllClientsQuery());
        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var client = await _mediator.Send(new GetClientByIdQuery(id));
            return Ok(client);
        }
        catch (ClientNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        try
        {
            var command = new CreateClientCommand(request.Name, request.Email, request.Phone);
            var client = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "VALIDATION_ERROR", message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequest request)
    {
        try
        {
            var command = new UpdateClientCommand(id, request.Name, request.Email, request.Phone);
            var client = await _mediator.Send(command);
            return client is null ? NotFound() : Ok(client);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "VALIDATION_ERROR", message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteClientCommand(id));
            return NoContent();
        }
        catch (ClientNotFoundException)
        {
            return NotFound();
        }
    }
}

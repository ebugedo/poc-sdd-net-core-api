using MediatR;
using Microsoft.AspNetCore.Mvc;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Application.Projects.Commands.CreateProject;
using poc_sdd_net_core_api.Application.Projects.Commands.DeleteProject;
using poc_sdd_net_core_api.Application.Projects.Commands.UpdateProject;
using poc_sdd_net_core_api.Application.Projects.Queries.GetAllProjects;
using poc_sdd_net_core_api.Application.Projects.Queries.GetProjectById;

namespace poc_sdd_net_core_api.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? clientId)
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery(clientId));
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(id));
            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        try
        {
            var command = new CreateProjectCommand(
                request.ClientId,
                request.Title,
                request.Description,
                request.Technologies,
                request.StartDate,
                request.DurationMonths);

            var project = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "VALIDATION_ERROR", message = ex.Message });
        }
        catch (ClientNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        try
        {
            var command = new UpdateProjectCommand(
                id,
                request.ClientId,
                request.Title,
                request.Description,
                request.Technologies,
                request.StartDate,
                request.DurationMonths);

            var project = await _mediator.Send(command);

            if (project is null)
                return NotFound();

            return Ok(project);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "VALIDATION_ERROR", message = ex.Message });
        }
        catch (ClientNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteProjectCommand(id));
            return NoContent();
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }
}

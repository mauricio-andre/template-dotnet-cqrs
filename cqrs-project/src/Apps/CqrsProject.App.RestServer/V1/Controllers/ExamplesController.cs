using Asp.Versioning;
using CqrsProject.App.RestServer.Filters;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Queries;
using CqrsProject.Core.Examples.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanReadExamples)]
[HeaderFilterTenantId]
public class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<ExampleResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchExampleQuery request)
    {
        var result = await _mediator.Send(request);
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExampleResponse), 200)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetExampleByKeyQuery(id));
        return Ok(result);
    }

    [HttpPost()]
    [ProducesResponseType(typeof(ExampleResponse), 201)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Create([FromBody] CreateExampleCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        await _mediator.Send(new RemoveExampleCommand(id));
        return NoContent();
    }
}

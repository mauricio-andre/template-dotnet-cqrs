using Asp.Versioning;
using CqrsProject.App.RestServer.Attributes;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Commands;
using CqrsProject.Core.Identity.Consts;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanReadExamples)]
[SwaggerHeaderTenantId]
public class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<ExampleResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Search([FromQuery] SearchExampleQuery request)
    {
        var result = await _mediator.Send(request);
        Response.Headers.AddCollectionHeaders(result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExampleResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetExampleByKeyQuery(id));
        return Ok(result);
    }

    [HttpPost()]
    [ProducesResponseType(typeof(ExampleResponse), 201)]
    [ProducesResponseType(400)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Create([FromBody] CreateExampleCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ExampleResponse), 204)]
    [ProducesResponseType(400)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        await _mediator.Send(new RemoveExampleCommand(id));
        return NoContent();
    }
}

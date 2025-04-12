using Asp.Versioning;
using CqrsProject.App.Attributes;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Queries;
using CqrsProject.Core.Examples.Responses;
using CqrsProject.Swagger.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.V1.Examples.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanReadExamples)]
[FromHeaderTenantId]
[HeaderFilterSwaggerTenantId]
public class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType<IList<ExampleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IList<ExampleResponse>>(StatusCodes.Status206PartialContent)]
    public async Task<IActionResult> Search([FromQuery] SearchExampleQuery request)
    {
        var result = await _mediator.Send(request);
        var list = await result.Items.ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);

        return StatusCode(
            result.TotalCount == list.Count
                ? StatusCodes.Status200OK
                : StatusCodes.Status206PartialContent,
            list
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ExampleResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetExampleByKeyQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<ExampleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Create([FromBody] CreateExampleCommand request)
    {
        var result = await _mediator.Send(request);
        var uri = Url.Action(nameof(Get), new { id = result.Id });

        return Created(uri, result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        await _mediator.Send(new RemoveExampleCommand(id));
        return NoContent();
    }
}

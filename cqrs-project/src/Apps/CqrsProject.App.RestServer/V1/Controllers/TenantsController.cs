using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Dtos;
using CqrsProject.Core.Commands;
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
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<TenantResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchTenantQuery request)
    {
        var result = await _mediator.Send(request);
        Response.Headers.AddCollectionHeaders(result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantResponse), 200)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetTenantByKeyQuery(id));
        return Ok(result);
    }

    [HttpPost()]
    [ProducesResponseType(typeof(TenantResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TenantResponse), 200)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateTenantDto request)
    {
        var result = await _mediator.Send(new UpdateTenantCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveTenantCommand(id));
        return NoContent();
    }
}

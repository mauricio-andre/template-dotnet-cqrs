using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Dtos;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.UserTenants.Queries;
using CqrsProject.Core.UserTenants.Responses;
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
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantResponse), 200)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetTenantByKeyQuery(id));
        return Ok(result);
    }

    [HttpPost]
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
        [FromBody] UpdateTenantRequestDto request)
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

    [HttpGet("{id}/users")]
    [ProducesResponseType(typeof(IList<UserTenantResponse>), 200)]
    public async Task<IActionResult> SearchUser(
        [FromRoute] Guid id,
        [FromQuery] SearchTenantUserTenantRequestDto request)
    {
        var result = await _mediator.Send(new SearchUserTenantQuery(
            UserName: request.UserName,
            TenantName: null,
            UserId: null,
            TenantId: id,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }
}

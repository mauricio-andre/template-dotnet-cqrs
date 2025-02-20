using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Tenants.Dtos;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.UserTenants.Commands;
using CqrsProject.Core.UserTenants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Tenants.Controllers;

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
        var list = await result.Items.ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
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
    [ProducesResponseType(typeof(IList<SearchUserTenantResponseDto>), 200)]
    public async Task<IActionResult> SearchUsers(
        [FromRoute] Guid id,
        [FromQuery] SearchUserTenantRequestDto request)
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

        var list = await result.Items
            .Select(item => new SearchUserTenantResponseDto(item.UserId, item.UserName))
            .ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
    }

    [HttpPost("{id}/users/{userId}")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateUsers([FromRoute] Guid id, [FromRoute] Guid userId)
    {
        await _mediator.Send(new CreateUserTenantCommand(userId, id));
        return Created();
    }

    [HttpDelete("{id}/users/{userId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveUsers([FromRoute] Guid id, [FromRoute] Guid userId)
    {
        await _mediator.Send(new RemoveUserTenantCommand(userId, id));
        return NoContent();
    }
}

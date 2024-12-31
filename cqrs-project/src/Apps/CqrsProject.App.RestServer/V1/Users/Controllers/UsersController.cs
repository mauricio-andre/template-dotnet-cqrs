using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Users.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.UserTenants.Commands;
using CqrsProject.Core.UserTenants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Users.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<UserResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchUserQuery request)
    {
        var result = await _mediator.Send(request);
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequestDto request)
    {
        var result = await _mediator.Send(new UpdateUserCommand(
            Id: id,
            UserName: request.UserName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber
        ));

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveUserCommand(id));
        return NoContent();
    }

    [HttpGet("{id}/roles")]
    [ProducesResponseType(typeof(IList<SearchUserRoleResponseDto>), 200)]
    public async Task<IActionResult> SearchRoles(
        [FromRoute] Guid id,
        [FromQuery] SearchUserRoleRequestDto request)
    {
        var result = await _mediator.Send(new SearchUserRoleQuery(
            UserId: id,
            RoleId: null,
            UserName: null,
            RoleName: request.RoleName,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));

        var dto = result.Items.Select(item => new SearchUserRoleResponseDto(item.RoleId, item.RoleName));

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await dto.ToListAsync());
    }

    [HttpPost("{id}/roles/{roleId}")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateRoles(
        [FromRoute] Guid id,
        [FromRoute] Guid roleId)
    {
        await _mediator.Send(new CreateUserRoleCommand(id, roleId));
        return Created();
    }

    [HttpDelete("{id}/roles/{roleId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveRoles(
        [FromRoute] Guid id,
        [FromRoute] Guid roleId)
    {
        await _mediator.Send(new RemoveUserRoleCommand(id, roleId));
        return NoContent();
    }

    [HttpGet("{id}/tenants")]
    [ProducesResponseType(typeof(IList<SearchUserTenantResponseDto>), 200)]
    public async Task<IActionResult> SearchTenants(
        [FromRoute] Guid id,
        [FromQuery] SearchUserTenantRequestDto request)
    {
        var result = await _mediator.Send(new SearchUserTenantQuery(
            UserId: id,
            TenantId: null,
            UserName: null,
            TenantName: request.TenantName,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));

        var dto = result.Items.Select(item => new SearchUserTenantResponseDto(item.TenantId, item.TenantName));

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await dto.ToListAsync());
    }

    [HttpPost("{id}/tenants/{tenantId}")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateTenants([FromRoute] Guid id, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new CreateUserTenantCommand(id, tenantId));
        return Created();
    }

    [HttpDelete("{id}/tenants/{tenantId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveTenants([FromRoute] Guid id, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RemoveUserTenantCommand(id, tenantId));
        return NoContent();
    }
}

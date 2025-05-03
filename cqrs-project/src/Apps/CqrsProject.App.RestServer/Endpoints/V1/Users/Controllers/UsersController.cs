using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.UserTenants.Commands;
using CqrsProject.Core.UserTenants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Controllers;

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
    [ProducesResponseType<IList<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IList<UserResponse>>(StatusCodes.Status206PartialContent)]
    public async Task<IActionResult> Search([FromQuery] SearchUserQuery request)
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
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand request)
    {
        var result = await _mediator.Send(request);
        var uri = Url.Action(nameof(Get), new { id = result.Id });
        return Created(uri, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveUserCommand(id));
        return NoContent();
    }

    [HttpGet("{id}/roles")]
    [ProducesResponseType<IList<SearchUserRoleResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IList<SearchUserRoleResponseDto>>(StatusCodes.Status206PartialContent)]
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

        var list = await result.Items
            .Select(item => new SearchUserRoleResponseDto(item.RoleId, item.RoleName))
            .ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);

        return StatusCode(
            result.TotalCount == list.Count
                ? StatusCodes.Status200OK
                : StatusCodes.Status206PartialContent,
            list
        );
    }

    [HttpPost("{id}/roles/{roleId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
    public async Task<IActionResult> CreateRoles(
        [FromRoute] Guid id,
        [FromRoute] Guid roleId)
    {
        await _mediator.Send(new CreateUserRoleCommand(id, roleId));
        return Created();
    }

    [HttpDelete("{id}/roles/{roleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveRoles(
        [FromRoute] Guid id,
        [FromRoute] Guid roleId)
    {
        await _mediator.Send(new RemoveUserRoleCommand(id, roleId));
        return NoContent();
    }

    [HttpGet("{id}/tenants")]
    [ProducesResponseType<IList<SearchUserTenantResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IList<SearchUserTenantResponseDto>>(StatusCodes.Status206PartialContent)]
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

        var list = await result.Items
            .Select(item => new SearchUserTenantResponseDto(item.TenantId, item.TenantName))
            .ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);

        return StatusCode(
            result.TotalCount == list.Count
                ? StatusCodes.Status200OK
                : StatusCodes.Status206PartialContent,
            list
        );
    }

    [HttpPost("{id}/tenants/{tenantId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
    public async Task<IActionResult> CreateTenants([FromRoute] Guid id, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new CreateUserTenantCommand(id, tenantId));
        return Created();
    }

    [HttpDelete("{id}/tenants/{tenantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveTenants([FromRoute] Guid id, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RemoveUserTenantCommand(id, tenantId));
        return NoContent();
    }
}

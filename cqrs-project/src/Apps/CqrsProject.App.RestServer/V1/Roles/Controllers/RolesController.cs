using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Roles.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Roles.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<RoleResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchRoleQuery request)
    {
        var result = await _mediator.Send(request);
        var list = await result.Items.ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleResponse), 200)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetRoleQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RoleResponse), 200)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateRoleRequestDto request)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveRoleCommand(id));
        return NoContent();
    }

    [HttpGet("{id}/users")]
    [ProducesResponseType(typeof(IList<SearchUserRoleResponseDto>), 200)]
    public async Task<IActionResult> SearchUsers(
        [FromRoute] Guid id,
        [FromQuery] SearchUserRoleRequestDto request)
    {
        var result = await _mediator.Send(new SearchUserRoleQuery(
            UserId: null,
            RoleId: id,
            UserName: request.UserName,
            RoleName: null,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));

        var list = await result.Items
            .Select(item => new SearchUserRoleResponseDto(item.UserId, item.UserName))
            .ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
    }

    [HttpPost("{id}/users/{userId}")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateUsers(
        [FromRoute] Guid id,
        [FromRoute] Guid userId)
    {
        await _mediator.Send(new CreateUserRoleCommand(userId, id));
        return Created();
    }

    [HttpDelete("{id}/users/{userId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveUsers(
        [FromRoute] Guid id,
        [FromRoute] Guid userId)
    {
        await _mediator.Send(new RemoveUserRoleCommand(userId, id));
        return NoContent();
    }
}

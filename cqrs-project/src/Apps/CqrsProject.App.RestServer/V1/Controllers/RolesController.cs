using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.UserRoles.Queries;
using CqrsProject.Core.UserRoles.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Controllers;

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
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
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
    [ProducesResponseType(typeof(IList<UserRoleResponse>), 200)]
    public async Task<IActionResult> SearchUsers(
        [FromRoute] Guid id,
        [FromQuery] SearchRoleUserRoleRequestDto request)
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

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }
}

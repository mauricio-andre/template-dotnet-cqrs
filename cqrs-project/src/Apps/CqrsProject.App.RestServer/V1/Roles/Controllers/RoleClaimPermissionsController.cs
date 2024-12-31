using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Roles.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Roles.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/roles/{roleId}/claim-permissions")]
[ControllerName("Roles")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class RoleClaimPermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoleClaimPermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<string>), 200)]
    public async Task<IActionResult> Search(
        [FromRoute] Guid roleId,
        [FromQuery] SearchRoleClaimPermissionRequestDto request)
    {
        var result = await _mediator.Send(new SearchRoleClaimPermissionQuery(
            RoleId: roleId,
            Name: request.Name,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid roleId,
        [FromBody] CreateRoleClaimPermissionRequestDto request)
    {
        await _mediator.Send(new CreateRoleClaimPermissionCommand(roleId, request.Name));
        return Ok();
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid roleId, [FromRoute] string name)
    {
        await _mediator.Send(new RemoveRoleClaimPermissionCommand(roleId, name));
        return NoContent();
    }
}

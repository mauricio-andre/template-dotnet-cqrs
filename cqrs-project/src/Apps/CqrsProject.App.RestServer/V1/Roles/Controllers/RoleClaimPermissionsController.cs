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
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status206PartialContent)]
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
        var list = await result.Items.ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);

        return StatusCode(
            result.TotalCount == list.Count
                ? StatusCodes.Status200OK
                : StatusCodes.Status206PartialContent,
            list
        );
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid roleId,
        [FromBody] CreateRoleClaimPermissionRequestDto request)
    {
        await _mediator.Send(new CreateRoleClaimPermissionCommand(roleId, request.Name));
        return Created();
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove([FromRoute] Guid roleId, [FromRoute] string name)
    {
        await _mediator.Send(new RemoveRoleClaimPermissionCommand(roleId, name));
        return NoContent();
    }
}

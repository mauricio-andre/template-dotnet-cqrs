using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.Core.UserRoles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CqrsProject.App.RestServer.V1.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class UserRolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserRolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] CreateUserRoleCommand request)
    {
        await _mediator.Send(request);
        return Created();
    }

    [HttpDelete("{userId}/{roleId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId)
    {
        await _mediator.Send(new RemoveUserRoleCommand(userId, roleId));
        return NoContent();
    }
}

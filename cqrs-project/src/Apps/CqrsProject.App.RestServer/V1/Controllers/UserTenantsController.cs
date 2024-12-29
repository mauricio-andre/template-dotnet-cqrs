using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.Core.UserTenants.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class UserTenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserTenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] CreateUserTenantCommand request)
    {
        await _mediator.Send(request);
        return Created();
    }

    [HttpDelete("{userId}/{tenantId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid userId, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RemoveUserTenantCommand(userId, tenantId));
        return NoContent();
    }
}

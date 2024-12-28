using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.Core.UserTenants.Commands;
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
public class UserTenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserTenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<UserTenantResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchUserTenantQuery request)
    {
        var result = await _mediator.Send(request);
        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await result.Items.ToListAsync());
    }

    [HttpPost()]
    [ProducesResponseType(typeof(UserTenantResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserTenantCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{userId}/{tenantId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove([FromRoute] Guid userId, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RemoveUserTenantCommand(userId, tenantId));
        return NoContent();
    }
}

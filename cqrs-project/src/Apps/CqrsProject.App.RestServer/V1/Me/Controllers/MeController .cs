using System.Security.Claims;
using Asp.Versioning;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.UserTenants.Queries;
using CqrsProject.Core.UserTenants.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Me.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
public class MeController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Sync()
    {
        await _mediator.Send(new IdentitySyncCommand(
            NameIdentifier: HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value,
            AccessToken: HttpContext.Request.Headers.Authorization.ToString().Split(" ")[1]));

        return NoContent();
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IList<MeTenantResponse>), 200)]
    public async Task<IActionResult> Tenants([FromQuery] SearchMeTenantQuery request)
    {
        var result = await _mediator.Send(request);
        var list = await result.Items.ToListAsync();

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IList<string>), 200)]
    public IActionResult Permissions()
    {
        var list = HttpContext.User.Identities
            .SelectMany(identity => identity.Claims)
            .Where(claim => claim.Type == AuthorizationPermissionClaims.ClaimType)
            .Select(claim => claim.Value)
            .Order();

        return Ok(list);
    }
}

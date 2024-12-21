using System.Security.Claims;
using Asp.Versioning;
using CqrsProject.Core.Identity.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Controllers;

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
}

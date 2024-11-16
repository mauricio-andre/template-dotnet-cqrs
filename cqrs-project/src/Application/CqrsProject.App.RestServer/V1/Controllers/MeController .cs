using System.Security.Claims;
using Asp.Versioning;
using CqrsProject.App.RestService.Extensions;
using CqrsProject.Core.Commands;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestService.V1.Controllers;

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

    [HttpPost("sync")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create()
    {
        await _mediator.Send(new IdentitySyncCommand(
            NameIdentifier: HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value,
            AccessToken: HttpContext.Request.Headers.Authorization.ToString().Split(" ")[1]));

        return NoContent();
    }
}

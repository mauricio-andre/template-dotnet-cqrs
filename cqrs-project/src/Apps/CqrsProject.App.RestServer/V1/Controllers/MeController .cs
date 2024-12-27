using System.Security.Claims;
using Asp.Versioning;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Dtos;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Queries;
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

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(IList<SearchMeTenantsResponseDto>), 200)]
    public async Task<IActionResult> Tenants([FromQuery] SearchMeTenantsRequestDto request)
    {
        var result = await _mediator.Send(new SearchUserTenantQuery(
            UserName: request.UserName,
            TenantName: request.TenantName,
            Take: request.Take,
            Skip: request.Skip,
            SortBy: request.SortBy
        ));

        var dto = result.Items.Select(item => new SearchMeTenantsResponseDto(
            Id: item.TenantId,
            Name: item.TenantName
        ));

        Response.Headers.AddContentRangeHeaders(request.Skip, request.Take, result.TotalCount);
        return Ok(await dto.ToListAsync());
    }
}

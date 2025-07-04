using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.Endpoints.V1.Tenants.Dtos;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.Endpoints.V1.Tenants.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/tenants/{tenantId}/connection-strings")]
[ControllerName("Tenants")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class TenantConnectionStringsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantConnectionStringsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType<IList<TenantConnectionStringResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IList<TenantConnectionStringResponse>>(StatusCodes.Status206PartialContent)]
    public async Task<IActionResult> Search(
        [FromRoute] Guid tenantId,
        [FromQuery] SearchTenantConnectionStringRequestDto request)
    {
        var result = await _mediator.Send(new SearchTenantConnectionStringQuery(
            TenantId: tenantId,
            ConnectionName: request.ConnectionName,
            Skip: request.Skip,
            Take: request.Take,
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

    [HttpGet("{id}")]
    [ProducesResponseType<TenantConnectionStringResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetTenantConnectionStringQuery(
            TenantId: tenantId,
            Id: id
        ));

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<TenantConnectionStringResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Application.ProblemJson)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid tenantId,
        [FromBody] CreateTenantConnectionStringRequestDto request)
    {
        var result = await _mediator.Send(new CreateTenantConnectionStringCommand(
            ConnectionName: request.ConnectionName,
            KeyName: request.KeyName,
            TenantId: tenantId
        ));

        var uri = Url.Action(nameof(Get), new { id = result.Id });

        return Created(uri, result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveTenantConnectionStringCommand(id, tenantId));
        return NoContent();
    }
}

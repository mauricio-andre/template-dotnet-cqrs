using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.V1.Tenants.Dtos;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProject.App.RestServer.V1.Tenants.Controllers;

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
    [ProducesResponseType(typeof(IList<TenantConnectionStringResponse>), 200)]
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
        Response.Headers.AddContentLengthHeaders(list.Count);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantConnectionStringResponse), 201)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid tenantId,
        [FromBody] CreateTenantConnectionStringRequestDto request)
    {
        var result = await _mediator.Send(new CreateTenantConnectionStringCommand(
            ConnectionName: request.ConnectionName,
            KeyName: request.KeyName,
            TenantId: tenantId
        ));

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Remove(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id)
    {
        await _mediator.Send(new RemoveTenantConnectionStringCommand(id, tenantId));
        return NoContent();
    }
}

using CqrsProject.Common.Loggers;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CqrsProject.App.RestServer.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(
        RequestDelegate next,
        ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        ICurrentIdentity currentIdentity,
        IMediator mediator)
    {
        if (!context.Request.Headers.TryGetValue("Tenant-Id", out var tenantIdHeader)
            || string.IsNullOrEmpty(tenantIdHeader))
        {
            await _next(context);
            return;
        }

        if (!Guid.TryParse(tenantIdHeader, out Guid tenantId))
        {
            context.Response.StatusCode = 403;
            return;
        }

        await mediator.Publish(new TenantAccessedByUserEvent(
            UserId: currentIdentity.GetLocalIdentityId(),
            TenantId: tenantId
        ));

        currentTenant.SetCurrentTenantId(tenantId);

        using (_logger.BeginScope(new TenantLoggerRecord(tenantId)))
            await _next(context);
    }
}

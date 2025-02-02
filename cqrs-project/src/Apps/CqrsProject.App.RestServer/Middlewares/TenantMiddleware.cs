using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Exceptions;
using CqrsProject.Core.Tenants.Interfaces;
using MediatR;

namespace CqrsProject.App.RestServer.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
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

        try
        {
            await mediator.Publish(new TenantAccessedByUserEvent(
                UserId: currentIdentity.GetLocalIdentityId(),
                TenantId: tenantId
            ));
        }
        catch (TenantUnreleasedException)
        {
            context.Response.StatusCode = 403;
            return;
        }

        using (currentTenant.BeginTenantScope(tenantId))
            await _next(context);
    }
}

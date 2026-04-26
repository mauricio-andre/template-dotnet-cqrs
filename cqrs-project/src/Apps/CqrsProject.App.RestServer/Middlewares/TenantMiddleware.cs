using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Tenants.Interfaces;

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
        ICurrentIdentity currentIdentity)
    {
        if (!context.Request.Headers.TryGetValue("Tenant-Id", out var tenantIdHeader)
            || string.IsNullOrEmpty(tenantIdHeader))
        {
            await _next(context);
            return;
        }

        if (!Guid.TryParse(tenantIdHeader, out Guid tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        if (!currentIdentity.GetTenants().Contains(tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        using (currentTenant.BeginTenantScope(tenantId))
            await _next(context);
    }
}

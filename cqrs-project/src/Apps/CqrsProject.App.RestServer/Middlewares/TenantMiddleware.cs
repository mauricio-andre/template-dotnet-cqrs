using CqrsProject.Core.Tenants;
using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestService.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant)
    {
        if (!context.Request.Headers.TryGetValue("x-tenant-id", out var tenantIdHeader)
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

        // TODO: implementar validação de permissão do usuário para o tenant

        currentTenant.SetCurrentTenantId(tenantId);
        await _next(context);
    }
}

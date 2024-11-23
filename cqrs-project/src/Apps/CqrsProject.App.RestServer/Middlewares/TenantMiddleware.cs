using CqrsProject.Common.Loggers;
using CqrsProject.Core.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CqrsProject.App.RestService.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

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

        using (_logger.BeginScope(new TenantLoggerRecord(tenantId)))
            await _next(context);
    }
}

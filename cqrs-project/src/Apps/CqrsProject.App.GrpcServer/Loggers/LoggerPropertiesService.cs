using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Loggers;
using CqrsProject.CustomConsoleFormatter.Interfaces;

namespace CqrsProject.App.GrpcServer.Loggers;

public class LoggerPropertiesService : ILoggerPropertiesService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggerPropertiesService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public string GetAppUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            var localId = user.Identities
                .FirstOrDefault(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
                ?.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            return localId ?? user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value!;
        }

        return "Unknown";
    }

    public KeyValuePair<string, object?>[] DefaultPropertyList() =>
        new TenantLoggerRecord().ToArray();

    public KeyValuePair<string, object?>[] ScopeObjectStructuring(object value)
    {
        if (value is TenantLoggerRecord tenantLoggerRecord)
            return tenantLoggerRecord.ToArray();

        return [];
    }
}

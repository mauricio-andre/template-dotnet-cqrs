using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Loggers;
using CqrsProject.CustomConsoleFormatter.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestServer.Loggers;

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
        LoggerPropertiesHelper.DefaultPropertyList();

    public KeyValuePair<string, object?>[] ScopeObjectStructuring(object value) =>
        LoggerPropertiesHelper.ScopeObjectStructuring(value);
}

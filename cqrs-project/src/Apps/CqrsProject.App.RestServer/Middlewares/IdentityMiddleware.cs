using System.Diagnostics;
using CqrsProject.Core.Identity.Interfaces;

namespace CqrsProject.App.RestServer.Middlewares;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentIdentity currentIdentity)
    {
        currentIdentity.SetCurrentIdentity(context.User);

        if (currentIdentity.HasLocalIdentity())
            Activity.Current?.AddTag("appUser", currentIdentity.GetLocalIdentityId());

        await _next(context);
    }
}

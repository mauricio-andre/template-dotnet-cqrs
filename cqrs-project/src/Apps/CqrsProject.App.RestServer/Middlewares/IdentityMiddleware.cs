using CqrsProject.Core.Identity;
using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestServer.Middlewares;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentIdentity currentIdentity)
    {
        currentIdentity.SetCurrentIdentity(context.User);
        await _next(context);
    }
}

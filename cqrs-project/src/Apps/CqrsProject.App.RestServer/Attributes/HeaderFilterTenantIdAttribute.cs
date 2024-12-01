using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CqrsProject.App.RestServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class HeaderFilterTenantIdAttribute : HeaderFilterAttribute
{
    public HeaderFilterTenantIdAttribute() : base("x-tenant-id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var value))
        {
            if (string.IsNullOrEmpty(value) && !AllowEmptyValue)
                context.Result = new BadRequestObjectResult($"header {HeaderName} can't be empty");

            return;
        }

        if (IsRequired)
            context.Result = new BadRequestObjectResult($"Missing required header: {HeaderName}");
    }
}

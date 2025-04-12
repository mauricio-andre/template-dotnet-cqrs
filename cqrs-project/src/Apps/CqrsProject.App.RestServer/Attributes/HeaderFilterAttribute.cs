using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CqrsProject.App.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FromHeaderFilterAttribute : ActionFilterAttribute
{
    public string HeaderName { get; }
    public string? Description { get; }
    public string? SchemaType { get; }
    public string? SchemaFormat { get; }
    public bool IsRequired { get; }
    public bool AllowEmptyValue { get; }

    public FromHeaderFilterAttribute(
        string headerName,
        string? description = null,
        string? schemaType = null,
        string? schemaFormat = null,
        bool isRequired = false,
        bool allowEmptyValue = false)
    {
        HeaderName = headerName;
        Description = description;
        SchemaType = schemaType;
        SchemaFormat = schemaFormat;
        IsRequired = isRequired;
        AllowEmptyValue = allowEmptyValue;
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

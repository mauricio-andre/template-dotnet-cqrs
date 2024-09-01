using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.App.RestService.Swagger;

public class HeadersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "x-tenant-id",
            In = ParameterLocation.Header,
            AllowEmptyValue = true,
            Required = false,
            Description = "Tenant Id in standard uuid format",
            Schema = new OpenApiSchema()
            {
                Type = "Guid"
            }
        });
    }
}

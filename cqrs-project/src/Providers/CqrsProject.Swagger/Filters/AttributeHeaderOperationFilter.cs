using System.Reflection;
using CqrsProject.Swagger.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.Swagger.Filters;

public class AttributeHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        var attributeList = context.MethodInfo.GetCustomAttributes<HeaderFilterSwaggerAttribute>();

        if (attributeList == null || !attributeList.Any())
            attributeList = context.MethodInfo.DeclaringType?.GetCustomAttributes<HeaderFilterSwaggerAttribute>();

        if (attributeList != null)
        {
            foreach (var attribute in attributeList)
            {
                var existingParam = operation.Parameters.FirstOrDefault(p =>
                    p.In == ParameterLocation.Header &&
                    p.Name == attribute.HeaderName);

                if (existingParam != null)
                    operation.Parameters.Remove(existingParam);

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = attribute.HeaderName,
                    In = ParameterLocation.Header,
                    Description = attribute.Description,
                    Required = attribute.IsRequired,
                    AllowEmptyValue = attribute.AllowEmptyValue,
                    Schema = string.IsNullOrEmpty(attribute.SchemaType)
                        ? null
                        : new OpenApiSchema
                        {
                            Type = attribute.SchemaType,
                            Format = attribute.SchemaFormat ?? string.Empty
                        }
                });
            }
        }
    }
}

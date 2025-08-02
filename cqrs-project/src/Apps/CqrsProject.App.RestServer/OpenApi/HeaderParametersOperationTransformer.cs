using CqrsProject.App.RestServer.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class HeaderParametersOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var attributeList = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<FromHeaderCustomAttribute>()
            .ToList();

        if (!attributeList.Any())
            return Task.CompletedTask;

        operation.Parameters ??= new List<OpenApiParameter>();

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

        return Task.CompletedTask;
    }
}

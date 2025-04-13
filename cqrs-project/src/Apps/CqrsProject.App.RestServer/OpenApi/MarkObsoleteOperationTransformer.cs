using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class MarkObsoleteOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var obsoleteAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<ObsoleteAttribute>()
            .FirstOrDefault();

        if (obsoleteAttribute != null)
        {
            operation.Deprecated = true;
            operation.Description = obsoleteAttribute.Message ?? operation.Description;
        }

        return Task.CompletedTask;
    }
}

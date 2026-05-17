using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class DefaultResponseOperationTransformer : IOpenApiOperationTransformer
{
    [SuppressMessage("Code Smell", "S2325", Justification = "N/A")]
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        operation.Responses ??= new OpenApiResponses();
        if (!operation.Responses.ContainsKey(StatusCodes.Status400BadRequest.ToString()))
            operation.Responses.Add(
                StatusCodes.Status400BadRequest.ToString(),
                new OpenApiResponse
                {
                    Description = HttpStatusCode.BadRequest.ToString(),
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            Application.ProblemJson,
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference(
                                    nameof(ProblemDetails),
                                    context.Document,
                                    null)
                            }
                        }
                    }
                });

        return Task.CompletedTask;
    }
}

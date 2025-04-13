using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class DefaultResponseOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
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
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = nameof(ProblemDetails)
                                    }
                                }
                            }
                        }
                    }
                });

        return Task.CompletedTask;
    }
}

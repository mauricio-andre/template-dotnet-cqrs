using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.Swagger.Filters;

public class DefaultResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var item in operation.Responses.Select(x => x.Value.Content))
        {
            var problemDetailsResponse = item
                .Where(x =>
                    x.Value.Schema.Reference?.Id == nameof(ProblemDetails))
                .ToList();

            for (int index = 0; index < problemDetailsResponse.Count; index++)
            {
                item.Remove(problemDetailsResponse[index]);
                item.Add(
                    Application.ProblemJson,
                    new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                    });
            }
        }

        if (!operation.Responses.ContainsKey(StatusCodes.Status400BadRequest.ToString()))
        {
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
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                            }
                        }
                    }
                });
        }
    }
}

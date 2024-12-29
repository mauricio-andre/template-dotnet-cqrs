using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.App.RestServer.Swagger;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
            return;

        operation.Responses.Add(
            StatusCodes.Status401Unauthorized.ToString(),
            new OpenApiResponse { Description = "Unauthorized" });

        operation.Responses.Add(
            StatusCodes.Status403Forbidden.ToString(),
            new OpenApiResponse { Description = "Forbidden" });

        operation.Responses.Add(
            StatusCodes.Status400BadRequest.ToString(),
            new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/problem+json", new OpenApiMediaType
                        {
                            Example = new OpenApiObject
                            {
                                ["type"] = new OpenApiString("string"),
                                ["title"] = new OpenApiString("string"),
                                ["status"] = new OpenApiInteger(StatusCodes.Status400BadRequest),
                                ["detail"] = new OpenApiString("string"),
                                ["instance"] = new OpenApiString("string"),
                                ["errors"] = new OpenApiObject
                                {
                                    ["additionalProp1"] = new OpenApiArray
                                    {
                                        new OpenApiString("string")
                                    },
                                    ["additionalProp2"] = new OpenApiArray
                                    {
                                        new OpenApiString("string")
                                    }
                                },
                                ["traceId"] = new OpenApiString("string"),
                                ["additionalProp1"] = new OpenApiString("string"),
                                ["additionalProp2"] = new OpenApiString("string")
                            }
                        }
                    }
                }
            });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    }
                ] = new List<string>()
            }
        };
    }
}

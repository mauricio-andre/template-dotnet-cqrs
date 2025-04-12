using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.Swagger.Filters;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authorizeAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>();
        if (!authorizeAttributes.Any())
            return;

        if (!operation.Responses.ContainsKey(StatusCodes.Status401Unauthorized.ToString()))
            operation.Responses.Add(
                StatusCodes.Status401Unauthorized.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });

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
                ] = [ "openid", "email", "offline_access" ]
            }
        };

        var hasPermission = authorizeAttributes.Any(authorize => authorize.Policy != null
            || authorize.Roles != null
            || authorize.AuthenticationSchemes != null);

        if (hasPermission && !operation.Responses.ContainsKey(StatusCodes.Status403Forbidden.ToString()))
            operation.Responses.Add(
                StatusCodes.Status403Forbidden.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Forbidden.ToString() });
    }
}

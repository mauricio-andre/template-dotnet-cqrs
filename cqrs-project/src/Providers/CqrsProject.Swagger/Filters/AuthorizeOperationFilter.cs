using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CqrsProject.Swagger.Filters;

public class AuthorizeOperationFilter : IOperationFilter
{
    [SuppressMessage("Code Smell", "S2325", Justification = "N/A")]
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authorizeAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>();
        if (!authorizeAttributes.Any())
            return;

        operation.Responses ??= new OpenApiResponses();

        if (!operation.Responses.ContainsKey(StatusCodes.Status401Unauthorized.ToString()))
            operation.Responses.Add(
                StatusCodes.Status401Unauthorized.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        SecuritySchemeType.OAuth2.GetDisplayName(),
                        context.Document)
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

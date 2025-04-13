using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class SecurityRequirementOperationTransformer(IConfiguration configuration) : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = context
            .Description
            .ActionDescriptor
            .EndpointMetadata
            .OfType<AuthorizeAttribute>();

        if (!authorizeAttributes.Any())
            return Task.CompletedTask;

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = SecuritySchemeType.OAuth2.GetDisplayName()
                    }
                }
            ] = configuration.GetValue<string>("OpenApi:Scopes")!.Split(" ")
        });

        if (!operation.Responses.ContainsKey(StatusCodes.Status401Unauthorized.ToString()))
            operation.Responses.Add(
                StatusCodes.Status401Unauthorized.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });

        var hasPermission = authorizeAttributes
            .Any(authorize => authorize.Policy != null
                || authorize.Roles != null
                || authorize.AuthenticationSchemes != null);

        if (hasPermission && !operation.Responses.ContainsKey(StatusCodes.Status403Forbidden.ToString()))
            operation.Responses.Add(
                StatusCodes.Status403Forbidden.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Forbidden.ToString() });

        return Task.CompletedTask;
    }
}

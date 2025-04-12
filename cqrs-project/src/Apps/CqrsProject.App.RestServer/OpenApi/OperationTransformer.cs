using System.Net;
using CqrsProject.App.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace CqrsProject.App.RestServer.OpenApi;

public static class OperationTransformer
{
    public static Func<OpenApiOperation, OpenApiOperationTransformerContext, CancellationToken, Task> AddMarkObsolete()
        => (operation, context, cancellationToken) =>
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
        };

    public static Func<OpenApiOperation, OpenApiOperationTransformerContext, CancellationToken, Task> AddHeaderParameters()
        => (operation, context, cancellationToken) =>
        {
            var attributeList = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<FromHeaderFilterTenantIdAttribute>()
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
        };

    public static Func<OpenApiOperation, OpenApiOperationTransformerContext, CancellationToken, Task> AddSecurityRequirement(
        IConfiguration configuration)
        => (operation, context, cancellationToken) =>
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
                            Id = JwtBearerDefaults.AuthenticationScheme
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
        };

    public static Func<OpenApiOperation, OpenApiOperationTransformerContext, CancellationToken, Task> AddDefaultResponse()
        => (operation, context, cancellationToken) =>
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
        };
}

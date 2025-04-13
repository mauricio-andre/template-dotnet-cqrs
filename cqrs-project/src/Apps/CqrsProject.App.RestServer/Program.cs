using Asp.Versioning;
using CqrsProject.App.RestServer.Authentication;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Extensions;
using CqrsProject.App.RestServer.Filters;
using CqrsProject.App.RestServer.Loggers;
using CqrsProject.App.RestServer.Middlewares;
using CqrsProject.App.RestServer.Transformers;
using CqrsProject.Auth0.Extensions;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Diagnostics;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Identity.Services;
using CqrsProject.Core.Tenants.Extensions;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.CustomCacheService.Extensions;
using CqrsProject.CustomConsoleFormatter.Extensions;
using CqrsProject.CustomStringLocalizer.Extensions;
using CqrsProject.OpenTelemetry.Extensions;
using CqrsProject.Postgres.Extensions;
using CqrsProject.Scalar.Extensions;
using CqrsProject.Swagger.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace CqrsProject.App.RestServer;

public class Program
{
    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddPostgresAdministrationDbContext()
            .AddPostgresCoreDbContext()
            .AddMediatR(config => config.RegisterServicesFromAssemblyContaining<CoreDbContext>())
            .Scan(scan => scan.FromAssembliesOf(typeof(CoreDbContext))
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime())
            .AddScoped<ITenantConnectionProvider, TenantConnectionProvider>()
            .AddScoped<ICurrentTenant, CurrentTenant>()
            .AddScoped<ICurrentIdentity, CurrentIdentity>()
            .AddSingleton(_ => new CqrsProjectActivitySource(builder.Configuration.GetValue<string>("ServiceName")!));

        // Configuration string location
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportCultures = builder
                .Configuration
                .GetValue<string>("SupportedCultures")!
                .Split(",")
                .Select(culture => culture.Trim())
                .ToArray();

            options
                .SetDefaultCulture(supportCultures[0])
                .AddSupportedCultures(supportCultures)
                .AddSupportedUICultures(supportCultures);
        });

        // configuration controllers
        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(
                        new KebabCaseParameterTransformer()));

                options.Filters.Add<ExceptionFilter>();
            });

        // configuration API Explorer
        builder.Services
            .AddEndpointsApiExplorer()
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApiVersions(builder.Services);

        // configuration cors
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var origins = builder
                    .Configuration
                    .GetValue<string>("Cors:AllowedOrigins")!
                    .Split(",")
                    .Select(origin => origin.Trim())
                    .ToArray();

                policy
                    .WithOrigins(origins)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .WithHeaders("Tenant-Id")
                    .WithExposedHeaders("Content-Range");
            });
        });

        // configuration identity
        builder.Services
            .AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AdministrationDbContext>();

        // configuration authentication
        builder.Services
            .AddAuthentication(schemes =>
            {
                schemes.DefaultAuthenticateScheme = AuthenticationDefaults.AuthenticationScheme;
                schemes.DefaultChallengeScheme = AuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("Authentication:Bearer:Authority");
                options.Audience = builder.Configuration.GetValue<string>("Authentication:Bearer:Audience");
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            })
            .AddScheme<AuthenticationOptions, AuthenticationHandler>(
                AuthenticationDefaults.AuthenticationScheme,
                AuthenticationDefaults.DisplayName,
                null);

        // configure authorization policies
        builder.Services.AddAuthorization(AuthorizationPolicyFactory.CreateDefaultPolicies());

        // Configure providers
        builder.Services.AddAuth0Provider(builder.Configuration);
        builder.Services.AddCustomCacheProvider();
        builder.Services.AddCustomStringLocalizerProvider();
        builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();
        builder.Services.AddSwaggerProvider(builder.Configuration);
        builder.AddOpenTelemetryProvider();

        var app = builder.Build();

        // configuration app
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors();
        app.MapControllers();
        app.UseStaticFiles();
        app.UseRequestLocalization();
        app.LoadMultiTenantConnections();
        app.MapOpenApi();

        // configuration swagger app
        app.UseSwaggerProvider();

        // configure Scalar
        app.UseScalarProvider(options =>
        {
            var clientId = app.Environment.IsDevelopment()
                ? app.Configuration.GetValue<string>("OpenApi:ClientId")
                : string.Empty;

            options
                .WithPreferredScheme(SecuritySchemeType.OAuth2.GetDisplayName())
                .WithOAuth2Authentication(oauth =>
                {
                    oauth.ClientId = clientId;
                    oauth.Scopes = app.Configuration.GetValue<string>("OpenApi:Scopes")!.Split(" ");
                })
                .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
        });

        app.UseMiddleware<IdentityMiddleware>();
        app.UseMiddleware<TenantMiddleware>();

        await app.RunAsync();
    }
}

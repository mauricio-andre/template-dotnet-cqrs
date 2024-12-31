using Asp.Versioning;
using CqrsProject.App.RestServer.Authentication;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.App.RestServer.Filters;
using CqrsProject.App.RestServer.Helpers;
using CqrsProject.App.RestServer.Loggers;
using CqrsProject.App.RestServer.Middlewares;
using CqrsProject.App.RestServer.Swagger;
using CqrsProject.App.RestServer.Transformers;
using CqrsProject.Auth0.Extensions;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Identity.Services;
using CqrsProject.Core.Tenants.Extensions;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.CustomConsoleFormatter.Extensions;
using CqrsProject.Postegre.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPostegreAdministrationDbContext(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("AdministrationDbContext"));
    })
    .AddPostegreCoreDbContext()
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
    .AddScoped<ICurrentIdentity, CurrentIdentity>();

// Configuration string location
builder.Services.AddLocalization(options => options.ResourcesPath = Path.Combine("Localization", "Resources"));
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

// Configure providers
builder.Services.AddAuth0Provider(builder.Configuration);
builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();

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
    });

// configuration swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => SwaggerSchemaIdHelper.GetSwaggerSchemaId(type));
});

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
    })
    .AddScheme<AuthenticationOptions, AuthenticationHandler>(
        AuthenticationDefaults.AuthenticationScheme,
        AuthenticationDefaults.DisplayName,
        null);

// configure authorization policies
builder.Services.AddAuthorization(AuthorizationPolicyFactory.CreateDefaultPolicies());

var app = builder.Build();

// configuration swagger app
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var descriptionList = app.DescribeApiVersions();
    foreach (var groupName in descriptionList.Select(desc => desc.GroupName))
    {
        var url = $"/swagger/{groupName}/swagger.json";
        var name = groupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }

    var clientId = builder.Configuration.GetValue<string>("PlatformSwagger:ClientId");
    var clientSecret = builder.Configuration.GetValue<string>("PlatformSwagger:ClientSecret");
    var audience = builder.Configuration.GetValue<string>("Authentication:Bearer:Audience")!;
    options.OAuthClientId(clientId);
    options.OAuthClientSecret(clientSecret);
    options.OAuthUsePkce();
    options.OAuthAdditionalQueryStringParams(new Dictionary<string, string>()
    {
        {"audience", audience}
    });

    options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});

// configuration app
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.UseStaticFiles();
app.UseRequestLocalization();
app.LoadMultiTenantConnections();

app.UseMiddleware<IdentityMiddleware>();
app.UseMiddleware<TenantMiddleware>();

await app.RunAsync();

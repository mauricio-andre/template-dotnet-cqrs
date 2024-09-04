using Asp.Versioning;
using CqrsProject.App.RestService.Authentication;
using CqrsProject.App.RestService.Middlewares;
using CqrsProject.App.RestService.Swagger;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Identity.Services;
using CqrsProject.Core.Tenants;
using CqrsProject.Postegre.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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
    .AddPostegreCoreDbContext(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("CoreDbContext"));
    })
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
    .AddScoped<IIdentitySyncService, IdentitySyncService>()
    .AddScoped<ICurrentTenant, CurrentTenant>();

// Configuration string location
builder.Services.AddLocalization();
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
builder.Services.AddControllers();

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
builder.Services.AddSwaggerGen();

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
            .WithHeaders("x-tenant-id")
            .WithExposedHeaders("x-total-count");
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
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                // Accept access_token inside cookies
                if (context.Request.Cookies.TryGetValue("access_token", out var token)
                    && context.Request.Headers.Authorization.ToString() == null)
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddScheme<AuthenticationOptions, AuthenticationHandler>(
        AuthenticationDefaults.AuthenticationScheme,
        "CqrsProject login",
        null);

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
});

// configuration app
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.UseStaticFiles();

app.UseMiddleware<LocalizationMiddleware>();
app.UseMiddleware<TenantMiddleware>();

await app.RunAsync();

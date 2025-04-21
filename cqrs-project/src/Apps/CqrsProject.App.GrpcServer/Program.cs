using CqrsProject.App.GrpcServer.Loggers;
using CqrsProject.Common.Diagnostics;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Identity.Services;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.OpenTelemetry.Extensions;
using CqrsProject.CustomCacheService.Extensions;
using CqrsProject.CustomConsoleFormatter.Extensions;
using CqrsProject.CustomStringLocalizer.Extensions;
using CqrsProject.Postgres.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CqrsProject.Core.Tenants.Extensions;
using CqrsProject.Auth0.Extensions;
using CqrsProject.Common.Consts;
using CqrsProject.App.GrpcServer.Authentication;
using CqrsProject.App.GrpcServer.Interceptors;
using CqrsProject.App.GrpcServer.V1.Me.Services;

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
builder.Services.AddAuthorization();
// builder.Services.AddAuthorization(AuthorizationPolicyFactory.CreateDefaultPolicies());

// Configure providers
builder.Services.AddAuth0Provider(builder.Configuration);
builder.Services.AddCustomCacheProvider();
builder.Services.AddCustomStringLocalizerProvider();
builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();
builder.AddOpenTelemetryProvider();

// Add gRPC
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<IdentityInterceptor>();
    options.Interceptors.Add<TenantInterceptor>();
});


builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.LoadMultiTenantConnections();

// Add gRPC Services
app.MapGrpcReflectionService();
app.MapGrpcService<MeService>();

await app.RunAsync();

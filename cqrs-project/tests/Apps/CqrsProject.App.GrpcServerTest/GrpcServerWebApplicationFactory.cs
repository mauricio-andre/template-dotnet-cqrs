using System.Text;
using CqrsProject.App.GrpcServer.Authentication;
using CqrsProject.Common.Consts;
using CqrsProject.Commons.Test.Database;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Core.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Grpc.Net.Client;
using NSubstitute;

namespace CqrsProject.App.GrpcServerTest;

public class GrpcServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    public IMediator Mediator => _mediator;

    public GrpcChannel CreateChannel()
        => GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions
            {
                HttpHandler = Server.CreateHandler()
            });

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(JwtHelper.Options);
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:AdministrationDbContext", "Administration" },
                { "ConnectionStrings:CoreDbContext", "Host" }
            });
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(service =>
                    service.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>)
                    || service.ServiceType == typeof(IDbContextFactory<AdministrationDbContext>)
                    || service.ServiceType == typeof(IDbContextFactory<CoreDbContext>)
                    || service.ServiceType == typeof(AdministrationDbContext)
                    || service.ServiceType == typeof(CoreDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = JwtHelper.Options["Authentication:Bearer:Authority"];
                options.Audience = JwtHelper.Options["Authentication:Bearer:Audience"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = JwtHelper.Options["Authentication:Bearer:Authority"],
                    ValidateAudience = true,
                    ValidAudience = JwtHelper.Options["Authentication:Bearer:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-secreta-mock-chave-secreta-mock-chave-secreta-mock")),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSingleton(_mediator);
            services.AddSingleton(new DbContextOptionsBuilder<CoreDbContext>().Options);
            services.AddSingleton(new DbContextOptionsBuilder<AdministrationDbContext>().Options);
            services.AddScoped<CoreDbContext>(sp => ActivatorUtilities.CreateInstance<SqliteCoreDbContext>(sp));
            services.AddScoped<AdministrationDbContext>(sp => ActivatorUtilities.CreateInstance<SqliteAdministrationDbContext>(sp));
            services.AddScoped<IDbContextFactory<CoreDbContext>, SqliteCoreDbContextFactory>();
            services.AddScoped<IDbContextFactory<AdministrationDbContext>, SqliteAdministrationDbContextFactory>();

            // Cria estrutura para o banco de dados
            var sqliteConnectionPull = new SqliteConnectionPull();
            services.AddSingleton(_ => sqliteConnectionPull);
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var coreDbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CoreDbContext>>();
                using var coreContext = coreDbContextFactory.CreateDbContext();
                coreContext.Database.EnsureCreated();

                var administrationDbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
                using var administrationContext = administrationDbContextFactory.CreateDbContext();
                administrationContext.Database.EnsureCreated();
            }
        });
    }
}

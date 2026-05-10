using CqrsProject.Commons.Test.Database;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CqrsProject.Core.Test.Infrastructure;

public sealed class CoreTestContext : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public CoreTestContext()
    {
        SqliteConnectionPull = new SqliteConnectionPull();
        TenantConnectionProvider = Substitute.For<ITenantConnectionProvider>();
        CurrentIdentity = Substitute.For<ICurrentIdentity>();
        Mediator = Substitute.For<IMediator>();
        Localizer = new TestStringLocalizer<CqrsProjectResource>();

        string coreConnectionKey = $"CoreDbContext-{Guid.NewGuid():N}";
        TenantConnectionProvider
            .GetConnectionStringToCurrentTenant(Arg.Any<string>())
            .Returns(coreConnectionKey);

        TenantConnectionProvider.LoadAllConnectionStringAsync().Returns(Task.CompletedTask);
        TenantConnectionProvider.IncludeConnectionStringAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.CompletedTask);

        string administrationConnectionKey = $"AdministrationDbContext-{Guid.NewGuid():N}";
        IConfigurationSection connectionStringsSection = Substitute.For<IConfigurationSection>();
        IConfigurationSection administrationConnectionSection = Substitute.For<IConfigurationSection>();
        IConfiguration configuration = Substitute.For<IConfiguration>();

        configuration.GetSection("ConnectionStrings").Returns(connectionStringsSection);
        connectionStringsSection.GetSection("AdministrationDbContext").Returns(administrationConnectionSection);
        connectionStringsSection["AdministrationDbContext"].Returns(administrationConnectionKey);
        administrationConnectionSection.Value.Returns(administrationConnectionKey);

        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton(SqliteConnectionPull);
        serviceCollection.AddSingleton(TenantConnectionProvider);
        serviceCollection.AddSingleton(CurrentIdentity);
        serviceCollection.AddSingleton(Mediator);
        serviceCollection.AddSingleton(Localizer);
        serviceCollection.AddSingleton(new DbContextOptionsBuilder<CoreDbContext>().Options);
        serviceCollection.AddSingleton(new DbContextOptionsBuilder<AdministrationDbContext>().Options);
        serviceCollection.AddScoped<CoreDbContext>(sp => ActivatorUtilities.CreateInstance<SqliteCoreDbContext>(sp));
        serviceCollection.AddScoped<AdministrationDbContext>(sp => ActivatorUtilities.CreateInstance<SqliteAdministrationDbContext>(sp));
        serviceCollection
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AdministrationDbContext>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        CoreDbContextFactory = new SqliteCoreDbContextFactory(_serviceProvider);
        AdministrationDbContextFactory = new SqliteAdministrationDbContextFactory(_serviceProvider);

        using CoreDbContext coreDbContext = CoreDbContextFactory.CreateDbContext();
        coreDbContext.Database.EnsureCreated();

        using AdministrationDbContext administrationDbContext = AdministrationDbContextFactory.CreateDbContext();
        administrationDbContext.Database.EnsureCreated();
    }

    public SqliteConnectionPull SqliteConnectionPull { get; }

    public ITenantConnectionProvider TenantConnectionProvider { get; }

    public ICurrentIdentity CurrentIdentity { get; }

    public IMediator Mediator { get; }

    public TestStringLocalizer<CqrsProjectResource> Localizer { get; }

    public UserManager<User> UserManager => _serviceProvider.GetRequiredService<UserManager<User>>();

    public RoleManager<IdentityRole<Guid>> RoleManager => _serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    public IDbContextFactory<CoreDbContext> CoreDbContextFactory { get; }

    public IDbContextFactory<AdministrationDbContext> AdministrationDbContextFactory { get; }

    public CoreDbContext CreateCoreDbContext() => CoreDbContextFactory.CreateDbContext();

    public AdministrationDbContext CreateAdministrationDbContext() => AdministrationDbContextFactory.CreateDbContext();

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}

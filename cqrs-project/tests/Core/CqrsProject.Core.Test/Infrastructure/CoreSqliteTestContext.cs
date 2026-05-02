using CqrsProject.Common.Localization;
using CqrsProject.Commons.Test.Database;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;
using NSubstitute;

namespace CqrsProject.Core.Test.Infrastructure;

internal sealed class CoreSqliteTestContext : IDisposable
{
    private readonly SqliteConnectionPull _connectionPull;
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public CoreSqliteTestContext()
    {
        _connectionPull = new SqliteConnectionPull();
        _tenantConnectionProvider = Substitute.For<ITenantConnectionProvider>();

        var connectionKey = $"CoreDbContext-{Guid.NewGuid():N}";
        _tenantConnectionProvider
            .GetConnectionStringToCurrentTenant(Arg.Any<string>())
            .Returns(connectionKey);

        _tenantConnectionProvider.LoadAllConnectionStringAsync().Returns(Task.CompletedTask);
        _tenantConnectionProvider.IncludeConnectionStringAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.CompletedTask);

        DbContext = new SqliteCoreDbContext(
            new DbContextOptionsBuilder<CoreDbContext>().Options,
            _tenantConnectionProvider,
            _connectionPull);

        DbContext.Database.EnsureCreated();
    }

    public CoreDbContext DbContext { get; }

    public IMediator Mediator { get; } = Substitute.For<IMediator>();

    public TestStringLocalizer<CqrsProjectResource> Localizer { get; } = new();

    public void Dispose()
    {
        DbContext.Dispose();
    }
}

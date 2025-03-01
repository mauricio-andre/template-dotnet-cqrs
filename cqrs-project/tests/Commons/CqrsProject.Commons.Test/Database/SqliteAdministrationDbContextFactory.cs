using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Commons.Test.Database;

public class SqliteAdministrationDbContextFactory : IDbContextFactory<AdministrationDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public SqliteAdministrationDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AdministrationDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<SqliteAdministrationDbContext>(_serviceProvider);

    public Task<AdministrationDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

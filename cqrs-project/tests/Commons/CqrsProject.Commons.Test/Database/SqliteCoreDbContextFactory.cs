using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Commons.Test.Database;

public class SqliteCoreDbContextFactory : IDbContextFactory<CoreDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public SqliteCoreDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CoreDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<SqliteCoreDbContext>(_serviceProvider);

    public Task<CoreDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

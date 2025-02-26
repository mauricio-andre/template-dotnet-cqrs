using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postgres.Data;

public class PostgresCoreDbContextFactory : IDbContextFactory<CoreDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public PostgresCoreDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CoreDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<PostgresCoreDbContext>(_serviceProvider);

    public Task<CoreDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

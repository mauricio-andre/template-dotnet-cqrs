using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postgres.Data;

public class PostgresAdministrationDbContextFactory : IDbContextFactory<AdministrationDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public PostgresAdministrationDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AdministrationDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<PostgresAdministrationDbContext>(_serviceProvider);

    public Task<AdministrationDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

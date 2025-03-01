using CqrsProject.Core.Data;
using CqrsProject.Postgres.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postgres.Extensions;

public static class PostgresServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresCoreDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = default)
    {
        return services
            .AddScoped<CoreDbContext, PostgresCoreDbContext>()
            .AddDbContextFactory<CoreDbContext, PostgresCoreDbContextFactory>(optionsAction, ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddPostgresAdministrationDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = default)
    {
        return services
            .AddScoped<AdministrationDbContext, PostgresAdministrationDbContext>()
            .AddDbContextFactory<AdministrationDbContext, PostgresAdministrationDbContextFactory>(optionsAction, ServiceLifetime.Scoped);
    }
}

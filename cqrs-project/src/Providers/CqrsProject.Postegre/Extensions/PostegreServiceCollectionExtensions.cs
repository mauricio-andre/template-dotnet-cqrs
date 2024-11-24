using CqrsProject.Core.Data;
using CqrsProject.Postegre.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postegre.Extensions;

public static class PostegreServiceCollectionExtensions
{
    public static IServiceCollection AddPostegreCoreDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        return services
            .AddScoped<CoreDbContext, PostegresCoreDbContext>()
            .AddDbContextFactory<CoreDbContext, PostegresCoreDbContextFactory>(optionsAction, ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddPostegreAdministrationDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        return services
            .AddScoped<AdministrationDbContext, PostegresAdministrationDbContext>()
            .AddDbContextFactory<AdministrationDbContext, PostegresAdministrationDbContextFactory>(optionsAction, ServiceLifetime.Scoped);
    }
}

using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postegre.Data;

public class PostegresAdministrationDbContextFactory : IDbContextFactory<AdministrationDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public PostegresAdministrationDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AdministrationDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<PostegresAdministrationDbContext>(_serviceProvider);

    public Task<AdministrationDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

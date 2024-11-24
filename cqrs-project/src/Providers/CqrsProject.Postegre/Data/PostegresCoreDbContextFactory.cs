using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Postegre.Data;

public class PostegresCoreDbContextFactory : IDbContextFactory<CoreDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public PostegresCoreDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CoreDbContext CreateDbContext() => ActivatorUtilities.CreateInstance<PostegresCoreDbContext>(_serviceProvider);

    public Task<CoreDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}

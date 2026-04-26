using CqrsProject.Common.Consts;
using CqrsProject.Common.Localization;
using CqrsProject.Common.Providers.Cache.Dtos;
using CqrsProject.Common.Providers.Cache.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CqrsProject.Core.Tenants.UseCases.TenantAccessedByUser;

namespace CqrsProject.Core.Tenants.Rules;

public class ShallNotAccessUnreleasedTenantRule : INotificationHandler<TenantAccessedByUserEvent>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly IChaceService _chaceService;

    public ShallNotAccessUnreleasedTenantRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        IChaceService chaceService)
    {
        _dbContextFactory = dbContextFactory;
        _stringLocalizer = stringLocalizer;
        _chaceService = chaceService;
    }

    public async Task Handle(TenantAccessedByUserEvent notification, CancellationToken cancellationToken)
    {
        var hasAccess = await _chaceService.GetOrCreateAsync(
            string.Format(CacheKeys.AccessUserTenantKey, notification.UserId, notification.TenantId),
            async () =>
            {
                var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
                return await administrationDbContext.UserTenants
                    .AnyAsync(userTenant => userTenant.TenantId == notification.TenantId
                        && userTenant.UserId == notification.UserId
                        && !userTenant.Tenant!.IsDeleted
                        && !userTenant.User!.IsDeleted,
                    cancellationToken);
            },
            new CacheEntryDto()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            }
        );

        if (!hasAccess)
            throw new TenantUnreleasedException(_stringLocalizer);
    }
}

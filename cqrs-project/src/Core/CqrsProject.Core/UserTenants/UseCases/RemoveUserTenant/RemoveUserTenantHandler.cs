using CqrsProject.Common.Consts;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Common.Providers.Cache.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.UserTenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.UserTenants.UseCases.RemoveUserTenant;

public class RemoveUserTenantHandler : IRequestHandler<RemoveUserTenantCommand>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<RemoveUserTenantCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly IChaceService _chaceService;

    public RemoveUserTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<RemoveUserTenantCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        IChaceService chaceService)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _chaceService = chaceService;
    }

    public async Task Handle(
        RemoveUserTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await administrationDbContext.UserTenants.FirstOrDefaultAsync(
            userTenant => userTenant.UserId == request.UserId
                && userTenant.TenantId == request.TenantId,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(UserTenant),
                string.Concat(
                    "userId: ",
                    request.UserId,
                    " tenantId: ",
                    request.TenantId
                ));

        _chaceService.TryRemove(string.Format(
            CacheKeys.AccessUserTenantKey,
            request.UserId,
            request.TenantId));

        administrationDbContext.Remove(entity);
        await administrationDbContext.SaveChangesAsync(cancellationToken);
    }
}

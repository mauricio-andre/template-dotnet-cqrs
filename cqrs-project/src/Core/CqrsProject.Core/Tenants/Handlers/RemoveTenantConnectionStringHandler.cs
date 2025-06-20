using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Handlers;

public class RemoveTenantConnectionStringHandler : IRequestHandler<RemoveTenantConnectionStringCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<RemoveTenantConnectionStringCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public RemoveTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<RemoveTenantConnectionStringCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        ITenantConnectionProvider tenantConnectionProvider)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _tenantConnectionProvider = tenantConnectionProvider;
    }

    public async Task Handle(
        RemoveTenantConnectionStringCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _administrationDbContext.TenantConnectionStrings.FirstOrDefaultAsync(
            entity => entity.Id == request.Id
                && entity.TenantId == request.TenantId,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(TenantConnectionString), request.Id.ToString());

        _administrationDbContext.Remove(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
        _tenantConnectionProvider.InvalidateConnectionString(entity.TenantId, entity.ConnectionName);
    }
}

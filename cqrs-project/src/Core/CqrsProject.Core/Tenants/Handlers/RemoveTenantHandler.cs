using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Handlers;

public class RemoveTenantHandler : IRequestHandler<RemoveTenantCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<RemoveTenantCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public RemoveTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<RemoveTenantCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        RemoveTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _administrationDbContext.Tenants.FirstOrDefaultAsync(
            tenant => tenant.Id == request.Id
                && !tenant.IsDeleted,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.Id.ToString());

        entity.IsDeleted = true;

        _administrationDbContext.Update(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
    }
}

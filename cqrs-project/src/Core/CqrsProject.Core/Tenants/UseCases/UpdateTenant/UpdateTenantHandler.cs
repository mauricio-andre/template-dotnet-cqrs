using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.UseCases.UpdateTenant;

public class UpdateTenantHandler : IRequestHandler<UpdateTenantCommand, TenantResponse>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<UpdateTenantCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public UpdateTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<UpdateTenantCommand> validator,
        IMediator mediator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
        _mediator = mediator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TenantResponse> Handle(
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await _mediator.Publish(new UpdateTenantEvent(request.Id, request.Name));

        var entity = await GetEntity(administrationDbContext, request, cancellationToken);

        entity.Name = request.Name;

        administrationDbContext.Update(entity);
        await administrationDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private async Task<Tenant> GetEntity(
        AdministrationDbContext administrationDbContext,
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await administrationDbContext.Tenants.FirstOrDefaultAsync(
            tenant => tenant.Id == request.Id
                && !tenant.IsDeleted,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.Id.ToString());

        return entity!;
    }

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

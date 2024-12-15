using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Commands;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Responses;
using CqrsProject.Core.Tenants;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Handlers;

public class UpdateTenantHandler : IRequestHandler<UpdateTenantCommand, TenantResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<UpdateTenantCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public UpdateTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<UpdateTenantCommand> validator,
        IMediator mediator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TenantResponse> Handle(
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new UpdateTenantEvent(request.Id, request.Name));

        var entity = await GetEntity(request, cancellationToken);

        entity.Name = request.Name;

        _administrationDbContext.Update(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private async Task<Tenant> GetEntity(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _administrationDbContext.Tenants.FirstOrDefaultAsync(
            tenant => tenant.Id == request.Id
                && !tenant.IsDeleted,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.Id.ToString());

        return entity;
    }

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

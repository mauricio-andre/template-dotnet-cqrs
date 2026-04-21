using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.Handlers;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, TenantResponse>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<CreateTenantCommand> _validator;
    private readonly IMediator _mediator;

    public CreateTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateTenantCommand> validator,
        IMediator mediator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<TenantResponse> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await _mediator.Publish(new CreateTenantEvent(request.Name));
        var entity = MapToEntity(request);
        administrationDbContext.Add(entity);
        await administrationDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private static Tenant MapToEntity(CreateTenantCommand request)
        => new Tenant { Name = request.Name };

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

using CqrsProject.Core.Commands;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Responses;
using CqrsProject.Core.Tenants;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Handlers;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, TenantResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<CreateTenantCommand> _validator;
    private readonly IMediator _mediator;

    public CreateTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateTenantCommand> validator,
        IMediator mediator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<TenantResponse> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateTenantEvent(request.Name));
        var entity = MapToEntity(request);
        _administrationDbContext.Add(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private static Tenant MapToEntity(CreateTenantCommand request)
        => new Tenant { Name = request.Name };

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

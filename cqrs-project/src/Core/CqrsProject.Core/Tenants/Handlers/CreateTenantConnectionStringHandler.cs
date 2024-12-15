using CqrsProject.Core.Commands;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Responses;
using CqrsProject.Core.Tenants;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Handlers;

public class CreateTenantConnectionStringHandler : IRequestHandler<
    CreateTenantConnectionStringCommand,
    TenantConnectionStringResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<CreateTenantConnectionStringCommand> _validator;
    private readonly IMediator _mediator;

    public CreateTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateTenantConnectionStringCommand> validator,
        IMediator mediator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<TenantConnectionStringResponse> Handle(
        CreateTenantConnectionStringCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateTenantConnectionStringEvent(
            request.TenantId,
            request.ConnectionName));

        var entity = MapToEntity(request);
        _administrationDbContext.Add(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private static TenantConnectionString MapToEntity(CreateTenantConnectionStringCommand request)
        => new TenantConnectionString
        {
            TenantId = request.TenantId,
            ConnectionName = request.ConnectionName,
            KeyName = request.KeyName
        };

    private static TenantConnectionStringResponse MapToResponse(TenantConnectionString entity)
        => new TenantConnectionStringResponse(
            entity.Id,
            entity.TenantId,
            entity.ConnectionName,
            entity.KeyName);
}

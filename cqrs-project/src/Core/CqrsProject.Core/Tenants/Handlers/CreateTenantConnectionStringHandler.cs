using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.Handlers;

public class CreateTenantConnectionStringHandler : IRequestHandler<
    CreateTenantConnectionStringCommand,
    TenantConnectionStringResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<CreateTenantConnectionStringCommand> _validator;
    private readonly IMediator _mediator;
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public CreateTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateTenantConnectionStringCommand> validator,
        IMediator mediator,
        ITenantConnectionProvider tenantConnectionProvider)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
        _tenantConnectionProvider = tenantConnectionProvider;
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

        await _tenantConnectionProvider.IncludeConnectionStringAsync(
            entity.TenantId,
            entity.ConnectionName,
            entity.KeyName);

        try
        {
            await _administrationDbContext.SaveChangesAsync(cancellationToken);
            return MapToResponse(entity);
        }
        catch (Exception)
        {
            _tenantConnectionProvider.InvalidateConnectionString(
                entity.TenantId,
                entity.ConnectionName);
            throw;
        }
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

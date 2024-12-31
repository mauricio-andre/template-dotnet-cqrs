using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.UserTenants.Commands;
using CqrsProject.Core.UserTenants.Entities;
using CqrsProject.Core.UserTenants.Events;
using CqrsProject.Core.Tenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CqrsProject.Core.Identity.Entities;

namespace CqrsProject.Core.UserTenants.Handlers;

public class CreateUserTenantHandler : IRequestHandler<CreateUserTenantCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<CreateUserTenantCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public CreateUserTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateUserTenantCommand> validator,
        IMediator mediator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        CreateUserTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateUserTenantEvent(request.UserId, request.TenantId));

        await CheckUserExistsAsync(request, cancellationToken);
        await CheckTenantExistsAsync(request, cancellationToken);
        var entity = MapToEntity(request);

        _administrationDbContext.Add(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CheckUserExistsAsync(CreateUserTenantCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _administrationDbContext.Users
            .Where(user => user.Id == request.UserId
                && !user.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!userExists)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.UserId.ToString());
    }

    private async Task CheckTenantExistsAsync(CreateUserTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantExists = await _administrationDbContext.Tenants
            .Where(tenant => tenant.Id == request.TenantId
                && !tenant.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!tenantExists)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.TenantId.ToString());
    }

    private static UserTenant MapToEntity(CreateUserTenantCommand request)
        => new UserTenant { UserId = request.UserId, TenantId = request.TenantId };
}

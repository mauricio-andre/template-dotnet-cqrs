using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Events;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Tenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateUserTenantHandler : IRequestHandler<CreateUserTenantCommand, UserTenantResponse>
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

    public async Task<UserTenantResponse> Handle(
        CreateUserTenantCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateUserTenantEvent(request.UserId, request.TenantId));

        var user = await GetUserAsync(request, cancellationToken);
        var tenant = await GetTenantAsync(request, cancellationToken);
        var entity = MapToEntity(request);

        _administrationDbContext.Add(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(entity, user, tenant);
    }

    private async Task<User> GetUserAsync(CreateUserTenantCommand request, CancellationToken cancellationToken)
    {
        var user = await _administrationDbContext.Users
            .Where(user => user.Id == request.UserId
                && !user.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.UserId.ToString());

        return user;
    }

    private async Task<Tenant> GetTenantAsync(CreateUserTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _administrationDbContext.Tenants
            .Where(tenant => tenant.Id == request.TenantId
                && !tenant.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.TenantId.ToString());

        return tenant;
    }

    private static UserTenant MapToEntity(CreateUserTenantCommand request)
        => new UserTenant { UserId = request.UserId, TenantId = request.TenantId };

    private static UserTenantResponse MapToResponse(UserTenant entity, User user, Tenant tenant)
        => new UserTenantResponse(
            entity.UserId,
            entity.TenantId,
            user.UserName!,
            tenant.Name
        );
}

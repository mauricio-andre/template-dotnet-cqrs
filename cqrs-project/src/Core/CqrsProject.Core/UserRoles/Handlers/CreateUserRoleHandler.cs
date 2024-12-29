using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.UserRoles.Commands;
using CqrsProject.Core.UserRoles.Events;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.UserRoles.Handlers;

public class CreateUserRoleHandler : IRequestHandler<CreateUserRoleCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<CreateUserRoleCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public CreateUserRoleHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<CreateUserRoleCommand> validator,
        IMediator mediator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _mediator = mediator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        CreateUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateUserRoleEvent(request.UserId, request.RoleId));

        await CheckUserExistsAsync(request, cancellationToken);
        await CheckRoleExistsAsync(request, cancellationToken);
        var entity = MapToEntity(request);

        _administrationDbContext.Add(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CheckUserExistsAsync(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _administrationDbContext.Users
            .Where(user => user.Id == request.UserId
                && !user.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!userExists)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.UserId.ToString());
    }

    private async Task CheckRoleExistsAsync(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var tenantExists = await _administrationDbContext.Tenants
            .Where(role => role.Id == request.RoleId)
            .AnyAsync(cancellationToken);

        if (tenantExists)
            throw new EntityNotFoundException(_stringLocalizer, nameof(IdentityRole<Guid>), request.RoleId.ToString());
    }

    private static IdentityUserRole<Guid> MapToEntity(CreateUserRoleCommand request)
        => new IdentityUserRole<Guid> { UserId = request.UserId, RoleId = request.RoleId };
}

using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class RemoveUserRoleHandler : IRequestHandler<RemoveUserRoleCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<RemoveUserRoleCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public RemoveUserRoleHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<RemoveUserRoleCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _administrationDbContext.UserRoles.FirstOrDefaultAsync(
            userRole => userRole.UserId == request.UserId
                && userRole.RoleId == request.RoleId,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(IdentityUserRole<Guid>),
                string.Concat(
                    "userId: ",
                    request.UserId,
                    " roleId: ",
                    request.RoleId
                ));

        _administrationDbContext.Remove(entity);
        await _administrationDbContext.SaveChangesAsync(cancellationToken);
    }
}

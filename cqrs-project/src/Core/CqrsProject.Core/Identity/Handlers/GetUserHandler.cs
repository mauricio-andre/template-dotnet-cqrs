using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserResponse>
{
    private readonly IValidator<GetUserQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;

    public GetUserHandler(
        IValidator<GetUserQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
    }

    public async Task<UserResponse> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _userManager.FindByIdAsync(request.Id.ToString());

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static UserResponse MapToResponse(User entity)
        => new UserResponse(
            entity.Id,
            entity.UserName!,
            entity.Email!,
            entity.PhoneNumber,
            entity.AccessFailedCount,
            entity.CreationTime,
            entity.LastModificationTime,
            entity.IsDeleted);
}

using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Events;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public UpdateUserHandler(
        IValidator<UpdateUserCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager,
        IMediator mediator)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<UserResponse> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new UpdateUserEvent(request.Id, request.UserName, request.Email));

        var entity = await _userManager.FindByIdAsync(request.Id.ToString());

        if (entity == null || entity.IsDeleted)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.Id.ToString());

        entity.EmailConfirmed = entity.EmailConfirmed
            && entity.NormalizedEmail == _userManager.NormalizeEmail(request.Email);
        entity.PhoneNumberConfirmed = entity.PhoneNumberConfirmed
            && entity.PhoneNumber == request.PhoneNumber;

        entity.UserName = request.UserName;
        entity.NormalizedUserName = _userManager.NormalizeName(request.UserName);
        entity.PhoneNumber = request.PhoneNumber;
        entity.Email = request.Email;
        entity.NormalizedEmail = _userManager.NormalizeEmail(request.Email);
        entity.LastModificationTime = DateTimeOffset.UtcNow;

        await _userManager.UpdateAsync(entity);

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
            entity.IsDeleted
        );
}

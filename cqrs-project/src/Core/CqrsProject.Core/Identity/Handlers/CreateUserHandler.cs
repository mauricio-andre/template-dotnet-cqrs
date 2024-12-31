using FluentValidation;
using MediatR;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.Commands;
using Microsoft.AspNetCore.Identity;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Events;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;

    public CreateUserHandler(
        IValidator<CreateUserCommand> validator,
        UserManager<User> userManager,
        IMediator mediator)
    {
        _validator = validator;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<UserResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateUserEvent(request.UserName, request.Email));

        var entity = MapToEntity(request);

        await _userManager.CreateAsync(entity);

        return MapToResponse(entity);
    }

    private User MapToEntity(CreateUserCommand request)
        => new User
        {
            UserName = request.UserName,
            NormalizedUserName = _userManager.NormalizeName(request.UserName),
            Email = request.Email,
            NormalizedEmail = _userManager.NormalizeEmail(request.Email),
            PhoneNumber = request.PhoneNumber,
            CreationTime = DateTimeOffset.UtcNow,
        };

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

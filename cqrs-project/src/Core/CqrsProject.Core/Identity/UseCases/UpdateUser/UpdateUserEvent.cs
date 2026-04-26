using MediatR;

namespace CqrsProject.Core.Identity.UseCases.UpdateUser;

public record UpdateUserEvent(
    Guid Id,
    string UserName,
    string Email
) : INotification;

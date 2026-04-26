using MediatR;

namespace CqrsProject.Core.Identity.UseCases.UpdateRole;

public record UpdateRoleEvent(
    Guid Id,
    string Name
) : INotification;

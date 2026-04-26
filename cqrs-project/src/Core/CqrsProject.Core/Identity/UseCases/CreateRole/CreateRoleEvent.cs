using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateRole;

public record CreateRoleEvent(
    string Name
) : INotification;

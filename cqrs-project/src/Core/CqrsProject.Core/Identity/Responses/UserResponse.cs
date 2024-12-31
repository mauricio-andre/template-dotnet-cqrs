namespace CqrsProject.Core.Identity.Responses;

public record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber,
    int AccessFailedCount,
    DateTimeOffset CreationTime,
    DateTimeOffset? LastModificationTime,
    bool IsDeleted
);

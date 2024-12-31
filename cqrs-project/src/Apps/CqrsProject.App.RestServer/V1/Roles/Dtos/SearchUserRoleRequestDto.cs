namespace CqrsProject.App.RestServer.V1.Roles.Dtos;

public record SearchUserRoleRequestDto(
    string? UserName,
    int? Take,
    int? Skip,
    string? SortBy
);

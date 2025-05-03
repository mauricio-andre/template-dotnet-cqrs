namespace CqrsProject.App.RestServer.Endpoints.V1.Roles.Dtos;

public record SearchRoleClaimPermissionRequestDto(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy
);

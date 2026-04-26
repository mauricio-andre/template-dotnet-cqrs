using FluentValidation;

namespace CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;

public class CreateUserTenantValidator : AbstractValidator<CreateUserTenantCommand>
{
    public CreateUserTenantValidator()
    {
        RuleFor(prop => prop.UserId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

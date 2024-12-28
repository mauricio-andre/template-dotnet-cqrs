using CqrsProject.Core.UserTenants.Commands;
using FluentValidation;

namespace CqrsProject.Core.UserTenants.Validators;

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

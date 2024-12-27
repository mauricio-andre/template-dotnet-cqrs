using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validators;

public class RemoveUserTenantValidator : AbstractValidator<RemoveUserTenantCommand>
{
    public RemoveUserTenantValidator()
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

using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class RemoveRoleValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

using CqrsProject.Core.Commands;
using CqrsProject.Core.Tenants;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(TenantConstrains.NameMaxLength);
    }
}

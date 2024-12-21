using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

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

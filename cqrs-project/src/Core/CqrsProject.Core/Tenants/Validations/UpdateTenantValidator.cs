using CqrsProject.Core.Commands;
using CqrsProject.Core.Tenants;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator()
    {
        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(TenantConstrains.NameMaxLength);
    }
}

using CqrsProject.Core.Tenants.Commands;
using CqrsProject.Core.Tenants.Entities;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(TenantConstrains.NameMaxLength);
    }
}

using CqrsProject.Core.Tenants.Entities;
using FluentValidation;

namespace CqrsProject.Core.Tenants.UseCases.CreateTenant;

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

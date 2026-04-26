using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.UserName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(prop => prop.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(prop => prop.PhoneNumber)
            .MaximumLength(20);
    }
}

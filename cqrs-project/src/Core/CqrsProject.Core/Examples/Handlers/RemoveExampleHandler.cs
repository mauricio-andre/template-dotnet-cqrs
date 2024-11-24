using CqrsProject.Common.Localization;
using CqrsProject.Core.Commands;
using CqrsProject.Core.Data;
using CqrsProject.Core.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Handlers;

public class RemoveExampleHandler : IRequestHandler<RemoveExampleCommand>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<RemoveExampleCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public RemoveExampleHandler(
        CoreDbContext coreDbContext,
        IValidator<RemoveExampleCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        RemoveExampleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _coreDbContext.Examples.FirstOrDefaultAsync(
            example => example.Id == request.Id,
            cancellationToken);

        if (entity == null)
            throw new ExampleNotFoundException(_stringLocalizer);

        _coreDbContext.Remove(entity);
        await _coreDbContext.SaveChangesAsync(cancellationToken);
    }
}

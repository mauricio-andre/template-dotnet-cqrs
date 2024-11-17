using CqrsProject.Core.Commands;
using CqrsProject.Core.Data;
using CqrsProject.Core.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Handlers;

public class RemoveExampleHandler : IRequestHandler<RemoveExampleCommand>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<RemoveExampleCommand> _validator;
    private readonly IMediator _mediator;

    public RemoveExampleHandler(
        CoreDbContext coreDbContext,
        IValidator<RemoveExampleCommand> validator,
        IMediator mediator)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task Handle(
        RemoveExampleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _coreDbContext.Examples.FirstOrDefaultAsync(
            example => example.Id == request.Id,
            cancellationToken);

        // TODO: configurar multi language
        if (entity == null)
            throw new ExampleNotFoundException("Definir messagem");

        _coreDbContext.Remove(entity);
        await _coreDbContext.SaveChangesAsync(cancellationToken);
    }
}

using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Handlers;

public class GetTenantByKeyHandler : IRequestHandler<GetTenantByKeyQuery, TenantResponse>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<GetTenantByKeyQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public GetTenantByKeyHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<GetTenantByKeyQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TenantResponse> Handle(
        GetTenantByKeyQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await administrationDbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                tenant => tenant.Id == request.Id
                    && !tenant.IsDeleted,
                cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

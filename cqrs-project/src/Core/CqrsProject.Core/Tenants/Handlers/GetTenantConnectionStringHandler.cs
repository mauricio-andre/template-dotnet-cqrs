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

namespace CqrsProject.Core.Handlers;

public class GetTenantConnectionStringHandler : IRequestHandler<
    GetTenantConnectionStringQuery,
    TenantConnectionStringResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<GetTenantConnectionStringQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;


    public GetTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<GetTenantConnectionStringQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TenantConnectionStringResponse> Handle(
        GetTenantConnectionStringQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _administrationDbContext.TenantConnectionStrings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity => entity.Id == request.Id
                    && entity.TenantId == request.TenantId,
                cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(TenantConnectionString), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static TenantConnectionStringResponse MapToResponse(TenantConnectionString entity)
        => new TenantConnectionStringResponse(
            entity.Id,
            entity.TenantId,
            entity.ConnectionName,
            entity.KeyName);
}

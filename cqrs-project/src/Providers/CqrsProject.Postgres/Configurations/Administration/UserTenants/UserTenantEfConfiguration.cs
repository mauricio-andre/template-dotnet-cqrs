using CqrsProject.Core.UserTenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postgres.Configurations.Administration;

public class UserTenantEfConfiguration : IEntityTypeConfiguration<UserTenant>
{
    public void Configure(EntityTypeBuilder<UserTenant> builder)
    {
        builder.ToTable("UserTenants");

        builder.HasKey(userTenant => new { userTenant.UserId, userTenant.TenantId });

        builder.Property(userTenant => userTenant.UserId);
        builder.Property(userTenant => userTenant.TenantId);
        builder.Property(userTenant => userTenant.CreationTime);
        builder.Property(userTenant => userTenant.CreatorId);
    }
}

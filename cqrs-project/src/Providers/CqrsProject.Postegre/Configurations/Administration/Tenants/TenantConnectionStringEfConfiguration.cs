using CqrsProject.Core.Tenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postegre.Configurations.Administration;

public class TenantConnectionStringEfConfiguration : IEntityTypeConfiguration<TenantConnectionString>
{
    public void Configure(EntityTypeBuilder<TenantConnectionString> builder)
    {
        builder.ToTable("TenantConnectionStrings");

        builder.HasKey(tenantConnectionString => tenantConnectionString.Id);

        builder
            .HasIndex(tenantConnectionString => new
            {
                tenantConnectionString.TenantId,
                tenantConnectionString.KeyName
            })
            .IsUnique();

        builder.Property(tenantConnectionString => tenantConnectionString.Id)
            .ValueGeneratedOnAdd();

        builder.Property(tenantConnectionString => tenantConnectionString.ConnectionName);

        builder.Property(tenantConnectionString => tenantConnectionString.KeyName);

        builder
            .HasOne(tenantConnectionString => tenantConnectionString.Tenant)
            .WithMany(tenant => tenant.TenantConnectionStringList)
            .HasForeignKey(tenantConnectionString => tenantConnectionString.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using CqrsProject.Core.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postegre.Configurations;

public class TenantConnectionStringEfConfiguration : IEntityTypeConfiguration<TenantConnectionString>
{
    public void Configure(EntityTypeBuilder<TenantConnectionString> builder)
    {
        builder.ToTable("TenantConnectionStrings");

        builder.HasKey(tenantConnectionString => tenantConnectionString.Id);

        builder.Property(tenantConnectionString => tenantConnectionString.Id)
            .ValueGeneratedOnAdd();

        builder.Property(tenantConnectionString => tenantConnectionString.ConnectionName)
            .IsRequired()
            .HasMaxLength(TenantConnectionStringConstrains.ConnectionNameMaxLength);

        builder.Property(tenantConnectionString => tenantConnectionString.KeyName)
            .IsRequired()
            .HasMaxLength(TenantConnectionStringConstrains.KeyNameMaxLength);

        builder
            .HasOne(tenantConnectionString => tenantConnectionString.Tenant)
            .WithMany(tenant => tenant.TenantConnectionStringList)
            .HasForeignKey(tenantConnectionString => tenantConnectionString.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

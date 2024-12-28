using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.UserTenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postegre.Configurations.Administration;

public class TenantEfConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(tenant => tenant.Id);

        builder.HasIndex(tenant => tenant.Name).IsUnique();

        builder.Property(tenant => tenant.Id)
            .ValueGeneratedOnAdd();

        builder.Property(tenant => tenant.Name)
            .IsRequired()
            .HasMaxLength(TenantConstrains.NameMaxLength);

        builder.Property(tenant => tenant.IsDeleted)
            .IsRequired();

        builder
            .HasMany(tenant => tenant.UserList)
            .WithMany(user => user.TenantList)
            .UsingEntity<UserTenant>(
                configureRight => configureRight
                    .HasOne(userTenant => userTenant.User)
                    .WithMany(user => user.UserTenantList)
                    .HasForeignKey(userTenant => userTenant.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                configureLeft => configureLeft
                    .HasOne(userTenant => userTenant.Tenant)
                    .WithMany(tenant => tenant.UserTenantList)
                    .HasForeignKey(userTenant => userTenant.TenantId)
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}

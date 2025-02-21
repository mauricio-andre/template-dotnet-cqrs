using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.Entities;

public static class TenantConnectionStringConstrains
{
    public const short ConnectionNameMaxLength = 50;
    public const short KeyNameMaxLength = 200;
}

public class TenantConnectionString
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Unicode(false)]
    [MaxLength(TenantConnectionStringConstrains.ConnectionNameMaxLength)]
    public string ConnectionName { get; set; } = string.Empty;

    [Required]
    [Unicode(false)]
    [MaxLength(TenantConnectionStringConstrains.KeyNameMaxLength)]
    public string KeyName { get; set; } = string.Empty;

    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}

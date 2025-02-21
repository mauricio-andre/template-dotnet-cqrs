using System.ComponentModel.DataAnnotations;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.UserTenants.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.Entities;

public static class TenantConstrains
{
    public const short NameMaxLength = 200;
}

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Unicode(false)]
    [MaxLength(TenantConstrains.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool IsDeleted { get; set; } = false;

    public ICollection<TenantConnectionString> TenantConnectionStringList { get; set; } = new List<TenantConnectionString>();
    public ICollection<UserTenant> UserTenantList { get; set; } = new List<UserTenant>();
    public ICollection<User> UserList { get; set; } = new List<User>();
}

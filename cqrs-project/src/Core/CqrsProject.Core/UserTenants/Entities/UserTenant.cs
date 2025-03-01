using System.ComponentModel.DataAnnotations;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.UserTenants.Entities;

public class UserTenant
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid TenantId { get; set; }

    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}

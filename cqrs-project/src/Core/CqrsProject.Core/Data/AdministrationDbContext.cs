using CqrsProject.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public abstract class AdministrationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    protected AdministrationDbContext(DbContextOptions options) : base(options)
    {
    }
}

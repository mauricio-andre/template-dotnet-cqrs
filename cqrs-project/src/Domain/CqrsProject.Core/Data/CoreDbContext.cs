using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public abstract class CoreDbContext : DbContext
{
    protected CoreDbContext(DbContextOptions options) : base(options)
    {
    }
}

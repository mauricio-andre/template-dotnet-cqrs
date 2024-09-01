using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Postegre.Data;

public class PostegresAdministrationDbContext : AdministrationDbContext
{
    public PostegresAdministrationDbContext(
        DbContextOptions<AdministrationDbContext> options) : base(options)
    {
    }
}

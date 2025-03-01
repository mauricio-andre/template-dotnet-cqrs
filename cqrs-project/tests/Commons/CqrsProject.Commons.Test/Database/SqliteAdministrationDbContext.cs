using CqrsProject.Core.Data;
using CqrsProject.Postgres.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CqrsProject.Commons.Test.Database;

public class SqliteAdministrationDbContext : PostgresAdministrationDbContext
{
    private readonly IConfiguration _configuration;
    public SqliteAdministrationDbContext(
        DbContextOptions<AdministrationDbContext> options,
        IConfiguration configuration) : base(options, configuration)
    {
        _configuration = configuration;
    }

    protected override void UseConnectionString(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            SqliteConnectionPull.Instance.GetOpenedConnection(
                _configuration.GetConnectionString("AdministrationDbContext")!
            ));
    }
}

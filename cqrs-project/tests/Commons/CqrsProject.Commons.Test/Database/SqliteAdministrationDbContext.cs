using CqrsProject.Core.Data;
using CqrsProject.Postgres.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CqrsProject.Commons.Test.Database;

public class SqliteAdministrationDbContext : PostgresAdministrationDbContext
{
    private readonly IConfiguration _configuration;
    private readonly SqliteConnectionPull _sqliteConnectionPull;
    public SqliteAdministrationDbContext(
        DbContextOptions<AdministrationDbContext> options,
        IConfiguration configuration,
        SqliteConnectionPull sqliteConnectionPull) : base(options, configuration)
    {
        _configuration = configuration;
        _sqliteConnectionPull = sqliteConnectionPull;
    }

    protected override void UseConnectionString(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            _sqliteConnectionPull.GetOpenedConnection(
                _configuration.GetConnectionString("AdministrationDbContext")!
            ));
    }
}

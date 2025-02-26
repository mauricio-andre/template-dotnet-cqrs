# Generate migrations

Criar migrations para o contexto de administração

```bash
dotnet ef migrations add Initials -p ./src/Providers/CqrsProject.Postgres -s ./src/Apps/CqrsProject.App.DbMigrator -o Migrations/AdministrationDbContextMigrations -c PostgresAdministrationDbContext
```

Criar migrations para o contexto do Core

```bash
dotnet ef migrations add Initials -p ./src/Providers/CqrsProject.Postgres -s ./src/Apps/CqrsProject.App.DbMigrator -o Migrations/CoreDbContextMigrations -c CoreDbContext
```

Executar as migrations

Execute o projeto DbMigrator ou use os comandos abaixo

```bash
dotnet ef database update -p ./src/Apps/CqrsProject.App.DbMigrator -c AdministrationDbContext

dotnet ef database update -p ./src/Apps/CqrsProject.App.DbMigrator -c CoreDbContext
```

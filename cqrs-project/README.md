# Generate migrations

Criar migrations para o contexto de administração

```bash
dotnet ef migrations add Initials -p ./src/Infrastructure/CqrsProject.Postegre -s ./src/Application/CqrsProject.App.DbMigrator -o Migrations/AdministrationDbContextMigrations -c PostegresAdministrationDbContext
```

Criar migrations para o contexto do Core

```bash
dotnet ef migrations add Initials -p ./src/Infrastructure/CqrsProject.Postegre -s ./src/Application/CqrsProject.App.DbMigrator -o Migrations/CoreDbContextMigrations -c PostegresCoreDbContext
```

Executar as migrations

Execute o projeto DbMigrator ou use os comandos abaixo

```bash
dotnet ef database update -p ./src/Application/CqrsProject.App.DbMigrator -c AdministrationDbContext

dotnet ef database update -p ./src/Application/CqrsProject.App.DbMigrator -c PostegresCoreDbContext
```

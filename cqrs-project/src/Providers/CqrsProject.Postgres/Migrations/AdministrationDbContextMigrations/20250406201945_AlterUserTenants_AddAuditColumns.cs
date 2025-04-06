using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CqrsProject.Postgres.Migrations.AdministrationDbContextMigrations
{
    /// <inheritdoc />
    public partial class AlterUserTenants_AddAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "UserTenants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "UserTenants",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "UserTenants");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "UserTenants");
        }
    }
}

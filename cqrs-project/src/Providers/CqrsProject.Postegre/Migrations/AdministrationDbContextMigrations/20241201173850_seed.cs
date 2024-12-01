using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CqrsProject.Postegre.Migrations.AdministrationDbContextMigrations
{
    /// <inheritdoc />
    public partial class seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("e83bfc7d-61af-ef11-b120-a830f9d53c51"), null, "host_admin", "HOST_ADMIN" });

            migrationBuilder.InsertData(
                table: "RoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { -1, "permissions", "manage_self", new Guid("e83bfc7d-61af-ef11-b120-a830f9d53c51") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoleClaims",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e83bfc7d-61af-ef11-b120-a830f9d53c51"));
        }
    }
}

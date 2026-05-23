using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CqrsProject.Postgres.Migrations.AdministrationDbContextMigrations
{
    /// <inheritdoc />
    public partial class dotnetUpdate10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e83bfc7d-61af-ef11-b120-a830f9d53c51"),
                column: "ConcurrencyStamp",
                value: "41f243ef-5fb9-4a2d-ac2d-e0a3c9ed1ed0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e83bfc7d-61af-ef11-b120-a830f9d53c51"),
                column: "ConcurrencyStamp",
                value: null);
        }
    }
}

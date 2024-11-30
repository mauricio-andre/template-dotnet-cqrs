using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CqrsProject.Postegre.Migrations.AdministrationDbContextMigrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("19d7fbe6-f687-4219-8fbd-92bfa2903ffe"), null, "tenant_admin", "TENANT_ADMIN" },
                    { new Guid("65e4f5a3-b56b-4a5a-9a0f-8483a875fc62"), null, "client", "CLIENT" },
                    { new Guid("d7d7a721-5c7c-474b-adfb-89b54cb3e3b0"), null, "host_admin", "HOST_ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("19d7fbe6-f687-4219-8fbd-92bfa2903ffe"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("65e4f5a3-b56b-4a5a-9a0f-8483a875fc62"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d7d7a721-5c7c-474b-adfb-89b54cb3e3b0"));
        }
    }
}

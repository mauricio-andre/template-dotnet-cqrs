using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CqrsProject.Postegre.Migrations.AdministrationDbContextMigrations
{
    /// <inheritdoc />
    public partial class CreateTable_TenantConnectionString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TenantConnectionStrings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectionName = table.Column<string>(type: "text", nullable: false),
                    KeyName = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantConnectionStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantConnectionStrings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantConnectionStrings_TenantId",
                table: "TenantConnectionStrings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantConnectionStrings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tenants");
        }
    }
}

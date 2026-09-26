using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorsAndProjectSector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sectors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sectors", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "sectors",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Administración pública" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Ingeniería" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Publicidad" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Servicios financieros" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Servicios tecnológicos" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Transporte" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Sector inmobiliario" }
                });

            migrationBuilder.CreateIndex(
                name: "idx_sectors_name",
                table: "sectors",
                column: "name",
                unique: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sector_id",
                table: "projects",
                type: "uuid",
                nullable: false,
                defaultValue: null);

            migrationBuilder.CreateIndex(
                name: "idx_projects_sector_id",
                table: "projects",
                column: "sector_id");

            migrationBuilder.AddForeignKey(
                name: "fk_projects_sector_id",
                table: "projects",
                column: "sector_id",
                principalTable: "sectors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_sector_id",
                table: "projects");

            migrationBuilder.DropTable(
                name: "sectors");

            migrationBuilder.DropIndex(
                name: "idx_projects_sector_id",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "sector_id",
                table: "projects");
        }
    }
}

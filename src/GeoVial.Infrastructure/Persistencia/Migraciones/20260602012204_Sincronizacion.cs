using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Sincronizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MarcaUltimaEdicion",
                table: "Comentario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CambioAplicado",
                columns: table => new
                {
                    CambioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AplicadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambioAplicado", x => x.CambioId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CambioAplicado_Relevamiento",
                table: "CambioAplicado",
                column: "RelevamientoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CambioAplicado");

            migrationBuilder.DropColumn(
                name: "MarcaUltimaEdicion",
                table: "Comentario");
        }
    }
}

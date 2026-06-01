using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Relevamientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Relevamiento",
                columns: table => new
                {
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentificacionObra = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    RadioAgrupacionMetros = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relevamiento", x => x.RelevamientoId);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionAgente",
                columns: table => new
                {
                    AsignacionAgenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgenteUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vigente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionAgente", x => x.AsignacionAgenteId);
                    table.ForeignKey(
                        name: "FK_AsignacionAgente_Relevamiento_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "Relevamiento",
                        principalColumn: "RelevamientoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_AsignacionAgente",
                table: "AsignacionAgente",
                columns: new[] { "RelevamientoId", "AgenteUsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Relevamiento_Area",
                table: "Relevamiento",
                column: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionAgente");

            migrationBuilder.DropTable(
                name: "Relevamiento");
        }
    }
}

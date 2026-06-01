using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Captura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Foto",
                columns: table => new
                {
                    FotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObservacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TieneMetadatosUbicacion = table.Column<bool>(type: "bit", nullable: false),
                    Fuente = table.Column<byte>(type: "tinyint", nullable: true),
                    ReferenciaArchivo = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foto", x => x.FotoId);
                });

            migrationBuilder.CreateTable(
                name: "Marcador",
                columns: table => new
                {
                    MarcadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    EnConflicto = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcador", x => x.MarcadorId);
                });

            migrationBuilder.CreateTable(
                name: "Observacion",
                columns: table => new
                {
                    ObservacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AgenteUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MomentoCaptura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SinGeorreferenciar = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observacion", x => x.ObservacionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Foto_Observacion",
                table: "Foto",
                column: "ObservacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Marcador_Relevamiento",
                table: "Marcador",
                column: "RelevamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Observacion_Marcador",
                table: "Observacion",
                column: "MarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Observacion_Relevamiento",
                table: "Observacion",
                column: "RelevamientoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Foto");

            migrationBuilder.DropTable(
                name: "Marcador");

            migrationBuilder.DropTable(
                name: "Observacion");
        }
    }
}

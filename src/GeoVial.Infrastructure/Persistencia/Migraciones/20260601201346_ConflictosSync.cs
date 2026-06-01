using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class ConflictosSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConflictoSync",
                columns: table => new
                {
                    ConflictoSyncId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<byte>(type: "tinyint", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecursosInvolucrados = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstadoResolucion = table.Column<byte>(type: "tinyint", nullable: false),
                    DecisorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictoSync", x => x.ConflictoSyncId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConflictoSync_Relevamiento_Estado",
                table: "ConflictoSync",
                columns: new[] { "RelevamientoId", "EstadoResolucion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConflictoSync");
        }
    }
}

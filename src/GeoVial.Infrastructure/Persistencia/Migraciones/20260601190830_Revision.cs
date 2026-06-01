using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Revision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comentario",
                columns: table => new
                {
                    ComentarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Momento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentario", x => x.ComentarioId);
                });

            migrationBuilder.CreateTable(
                name: "ComentarioEtiqueta",
                columns: table => new
                {
                    ComentarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EtiquetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioEtiqueta", x => new { x.ComentarioId, x.EtiquetaId });
                });

            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    EtiquetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiqueta", x => x.EtiquetaId);
                });

            migrationBuilder.CreateTable(
                name: "FotoEtiqueta",
                columns: table => new
                {
                    FotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EtiquetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoEtiqueta", x => new { x.FotoId, x.EtiquetaId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Foto_Marcador",
                table: "Foto",
                column: "MarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_Marcador",
                table: "Comentario",
                column: "MarcadorId");

            migrationBuilder.CreateIndex(
                name: "UX_Etiqueta_Nombre",
                table: "Etiqueta",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentario");

            migrationBuilder.DropTable(
                name: "ComentarioEtiqueta");

            migrationBuilder.DropTable(
                name: "Etiqueta");

            migrationBuilder.DropTable(
                name: "FotoEtiqueta");

            migrationBuilder.DropIndex(
                name: "IX_Foto_Marcador",
                table: "Foto");
        }
    }
}

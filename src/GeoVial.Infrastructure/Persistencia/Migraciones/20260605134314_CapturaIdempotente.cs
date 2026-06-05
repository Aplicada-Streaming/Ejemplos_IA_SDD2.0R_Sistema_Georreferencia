using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class CapturaIdempotente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CapturaId",
                table: "Observacion",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_Observacion_CapturaId",
                table: "Observacion",
                column: "CapturaId",
                unique: true,
                filter: "[CapturaId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Observacion_CapturaId",
                table: "Observacion");

            migrationBuilder.DropColumn(
                name: "CapturaId",
                table: "Observacion");
        }
    }
}

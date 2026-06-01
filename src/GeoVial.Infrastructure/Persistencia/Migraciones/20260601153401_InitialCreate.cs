using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JefeAreaUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "Credencial",
                columns: table => new
                {
                    CredencialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HashClave = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credencial", x => x.CredencialId);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAuditoria",
                columns: table => new
                {
                    RegistroAuditoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Momento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Operacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecursoAfectado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAuditoria", x => x.RegistroAuditoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RolJerarquico = table.Column<byte>(type: "tinyint", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EstadoVigencia = table.Column<bool>(type: "bit", nullable: false),
                    MetodoSeguridadConfigurado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                });

            migrationBuilder.CreateIndex(
                name: "UX_Credencial_NombreUsuario",
                table: "Credencial",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_Momento",
                table: "RegistroAuditoria",
                column: "Momento");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Area",
                table: "Usuario",
                column: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Credencial");

            migrationBuilder.DropTable(
                name: "RegistroAuditoria");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}

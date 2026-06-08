using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.Infrastructure.Persistencia;

/// <summary>
/// Seed de datos de desarrollo/demo (solo Development): crea la jerarquía de prueba (jefe general,
/// jefa de área y tres agentes de campo), los relevamientos de muestra y sus asignaciones, replicando
/// DATOS-DE-PRUEBA.md. Supera a <see cref="SeedDemo"/> (que sólo creaba el relevamiento demo sin agentes).
/// <para>
/// Idempotente: cada entidad se crea sólo si falta —los usuarios se identifican por su nombre de
/// credencial y los relevamientos por su identificación de obra—, de modo que sea seguro ejecutarlo en
/// cada arranque sobre una base persistente (SQL Server), sin duplicar.
/// </para>
/// </summary>
public static class SeedDesarrollo
{
    private sealed record UsuarioSeed(string Nombre, RolJerarquico Rol, bool ConArea, string Usuario, string Clave);

    private static readonly UsuarioSeed[] UsuariosDemo =
    {
        new("Jorge (Jefe General)", RolJerarquico.JefeGeneral, ConArea: false, "jefe.general", "JefeGeneral.2026"),
        new("Norma (Jefa de Zona Norte)", RolJerarquico.JefeArea, ConArea: true, "jefe.norte", "JefeNorte.2026"),
        new("Carlos (Agente)", RolJerarquico.AgenteCampo, ConArea: true, "campo1", "Campo1.2026"),
        new("Ana (Agente)", RolJerarquico.AgenteCampo, ConArea: true, "campo2", "Campo2.2026"),
        new("Diego (Agente)", RolJerarquico.AgenteCampo, ConArea: true, "campo3", "Campo3.2026"),
    };

    public static async Task EjecutarAsync(GeoVialDbContext db, IHasherClave hasher, CancellationToken ct = default)
    {
        // SeedInicial siembra el área "Zona Norte"; sin ella no hay dónde colgar a los usuarios de área.
        var area = await db.Areas.FirstOrDefaultAsync(ct);
        if (area is null)
        {
            return;
        }

        // 1) Usuarios + credenciales (idempotente por nombre de credencial).
        foreach (var u in UsuariosDemo)
        {
            if (await db.Credenciales.AnyAsync(c => c.NombreUsuario == u.Usuario, ct))
            {
                continue;
            }

            var usuario = Usuario.Crear(u.Nombre, u.Rol, u.ConArea ? area.AreaId : null).Valor!;
            await db.Usuarios.AddAsync(usuario, ct);
            await db.Credenciales.AddAsync(new Credencial(usuario.UsuarioId, u.Usuario, hasher.Hash(u.Clave)), ct);
        }

        await db.SaveChangesAsync(ct);

        // 2) Relevamientos + asignaciones (idempotente por identificación de obra).
        var campo1 = await AgentePorCredencialAsync(db, "campo1", ct);
        var campo2 = await AgentePorCredencialAsync(db, "campo2", ct);

        await CrearRelevamientoSiFaltaAsync(db, "Obra demo — Puente Río 12", 15m, area.AreaId, new[] { campo1 }, ct);
        await CrearRelevamientoSiFaltaAsync(db, "Ruta 8 km 45 — alcantarilla", 15m, area.AreaId, new[] { campo2 }, ct);
        await CrearRelevamientoSiFaltaAsync(db, "Túnel Acceso Sur — fisuras", 20m, area.AreaId, new[] { campo1, campo2 }, ct);
    }

    private static async Task<Usuario> AgentePorCredencialAsync(GeoVialDbContext db, string nombreUsuario, CancellationToken ct)
    {
        var credencial = await db.Credenciales.FirstAsync(c => c.NombreUsuario == nombreUsuario, ct);
        return await db.Usuarios.FirstAsync(u => u.UsuarioId == credencial.UsuarioId, ct);
    }

    private static async Task CrearRelevamientoSiFaltaAsync(
        GeoVialDbContext db, string obra, decimal radioMetros, Guid areaId, Usuario[] agentes, CancellationToken ct)
    {
        if (await db.Relevamientos.AnyAsync(r => r.IdentificacionObra == obra, ct))
        {
            return;
        }

        var relevamiento = Relevamiento.Crear(obra, radioMetros, areaId).Valor!;
        foreach (var agente in agentes)
        {
            relevamiento.AsignarAgente(agente);
        }

        await db.Relevamientos.AddAsync(relevamiento, ct);
        await db.SaveChangesAsync(ct);
    }
}

using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.Infrastructure.Persistencia;

/// <summary>
/// Seed de arranque (BT-10): crea el usuario raíz y un área inicial para configurar el primer eslabón
/// de la jerarquía (CU-03 §10). Idempotente: no duplica si ya existe el raíz.
/// </summary>
public static class SeedInicial
{
    public const string UsuarioRaiz = "raiz";
    public const string ClaveRaizPorDefecto = "GeoVial.Raiz.2026";

    public static async Task EjecutarAsync(GeoVialDbContext db, IHasherClave hasher, CancellationToken ct = default)
    {
        if (db.Database.IsRelational())
        {
            // Aplica las migraciones de EF Core sobre la base local (ADR-09, migración 20260601_InitialCreate).
            await db.Database.MigrateAsync(ct);
        }

        if (await db.Usuarios.AnyAsync(u => u.Rol == RolJerarquico.Raiz, ct))
        {
            return;
        }

        var area = Area.Crear("Zona Norte");
        await db.Areas.AddAsync(area, ct);

        var raizResultado = Usuario.Crear("Administrador técnico", RolJerarquico.Raiz, areaId: null);
        var raiz = raizResultado.Valor!;
        await db.Usuarios.AddAsync(raiz, ct);
        await db.Credenciales.AddAsync(new Credencial(raiz.UsuarioId, UsuarioRaiz, hasher.Hash(ClaveRaizPorDefecto)), ct);

        await db.SaveChangesAsync(ct);
    }
}

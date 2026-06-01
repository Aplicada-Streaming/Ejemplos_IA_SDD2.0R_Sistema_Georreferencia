using GeoVial.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.Infrastructure.Persistencia;

/// <summary>
/// Contexto EF Core del backend (ADR-09). Mapea el slice de jerarquía y acceso del modelo lógico:
/// Usuario (§1.1), Area (§1.2), RegistroAuditoria (§1.12) y la Credencial de soporte de ROPC.
/// El resto de las 12 entidades se incorpora en BT-07 (EP-T2).
/// </summary>
public sealed class GeoVialDbContext : DbContext
{
    public GeoVialDbContext(DbContextOptions<GeoVialDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Credencial> Credenciales => Set<Credencial>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();
    public DbSet<Relevamiento> Relevamientos => Set<Relevamiento>();
    public DbSet<Marcador> Marcadores => Set<Marcador>();
    public DbSet<Observacion> Observaciones => Set<Observacion>();
    public DbSet<Foto> Fotos => Set<Foto>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        modelo.Entity<Usuario>(e =>
        {
            e.ToTable("Usuario");
            e.HasKey(u => u.UsuarioId);
            e.Property(u => u.Nombre).HasMaxLength(200).IsRequired();
            e.Property(u => u.Rol).HasConversion<byte>().HasColumnName("RolJerarquico").IsRequired();
            e.Property(u => u.EstadoVigencia).IsRequired();
            e.Property(u => u.MetodoSeguridadConfigurado).IsRequired();
            e.HasIndex(u => u.AreaId).HasDatabaseName("IX_Usuario_Area");
        });

        modelo.Entity<Area>(e =>
        {
            e.ToTable("Area");
            e.HasKey(a => a.AreaId);
            e.Property(a => a.Nombre).HasMaxLength(200).IsRequired();
        });

        modelo.Entity<Credencial>(e =>
        {
            e.ToTable("Credencial");
            e.HasKey(c => c.CredencialId);
            e.Property(c => c.NombreUsuario).HasMaxLength(200).IsRequired();
            e.Property(c => c.HashClave).HasMaxLength(1024).IsRequired();
            e.HasIndex(c => c.NombreUsuario).IsUnique().HasDatabaseName("UX_Credencial_NombreUsuario");
        });

        modelo.Entity<RegistroAuditoria>(e =>
        {
            e.ToTable("RegistroAuditoria");
            e.HasKey(r => r.RegistroAuditoriaId);
            e.Property(r => r.Operacion).HasMaxLength(200).IsRequired();
            e.Property(r => r.RecursoAfectado).HasMaxLength(300).IsRequired();
            e.HasIndex(r => r.Momento).HasDatabaseName("IX_Auditoria_Momento");
        });

        modelo.Entity<Relevamiento>(e =>
        {
            e.ToTable("Relevamiento");
            e.HasKey(r => r.RelevamientoId);
            e.Property(r => r.IdentificacionObra).HasMaxLength(300).IsRequired();
            e.Property(r => r.Estado).HasConversion<byte>().IsRequired();
            e.Property(r => r.RadioAgrupacionMetros).HasColumnType("decimal(9,2)").IsRequired();
            e.HasIndex(r => r.AreaId).HasDatabaseName("IX_Relevamiento_Area");
            e.HasMany(r => r.Asignaciones).WithOne().HasForeignKey(a => a.RelevamientoId).OnDelete(DeleteBehavior.Cascade);
            e.Navigation(r => r.Asignaciones).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelo.Entity<AsignacionAgente>(e =>
        {
            e.ToTable("AsignacionAgente");
            e.HasKey(a => a.AsignacionAgenteId);
            e.Property(a => a.Vigente).IsRequired();
            e.HasIndex(a => new { a.RelevamientoId, a.AgenteUsuarioId }).IsUnique().HasDatabaseName("UX_AsignacionAgente");
        });

        modelo.Entity<Marcador>(e =>
        {
            e.ToTable("Marcador");
            e.HasKey(m => m.MarcadorId);
            e.Ignore(m => m.Coordenada);
            e.Property(m => m.Latitud).HasColumnType("decimal(9,6)").IsRequired();
            e.Property(m => m.Longitud).HasColumnType("decimal(9,6)").IsRequired();
            e.Property(m => m.EnConflicto).IsRequired();
            e.HasIndex(m => m.RelevamientoId).HasDatabaseName("IX_Marcador_Relevamiento");
        });

        modelo.Entity<Observacion>(e =>
        {
            e.ToTable("Observacion");
            e.HasKey(o => o.ObservacionId);
            e.Property(o => o.MomentoCaptura).IsRequired();
            e.Property(o => o.SinGeorreferenciar).IsRequired();
            e.HasIndex(o => o.RelevamientoId).HasDatabaseName("IX_Observacion_Relevamiento");
            e.HasIndex(o => o.MarcadorId).HasDatabaseName("IX_Observacion_Marcador");
        });

        modelo.Entity<Foto>(e =>
        {
            e.ToTable("Foto");
            e.HasKey(f => f.FotoId);
            e.Property(f => f.ReferenciaArchivo).HasMaxLength(1024).IsRequired();
            e.Property(f => f.TieneMetadatosUbicacion).IsRequired();
            e.Property(f => f.Fuente).HasConversion<byte?>();
            e.HasIndex(f => f.ObservacionId).HasDatabaseName("IX_Foto_Observacion");
        });
    }
}

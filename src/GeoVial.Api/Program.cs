using System.Security.Claims;
using System.Text;
using GeoVial.Api;
using GeoVial.Application;
using GeoVial.Application.Captura;
using GeoVial.Application.Conflictos;
using GeoVial.Application.Cqrs;
using GeoVial.Application.ExportImport;
using GeoVial.Application.Relevamientos;
using GeoVial.Application.Revision;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using GeoVial.Infrastructure;
using GeoVial.Infrastructure.Persistencia;
using GeoVial.Infrastructure.Seguridad;
using GeoVial.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection(JwtOptions.Seccion).Get<JwtOptions>() ?? new JwtOptions();
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.ClaveSecreta)),
            RoleClaimType = "role",
            NameClaimType = "sub",
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed de arranque (BT-10): usuario raíz y área inicial.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<GeoVial.Application.Abstracciones.IHasherClave>();
    await SeedInicial.EjecutarAsync(db, hasher);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

// --- Acceso (CU-02; US-04, US-05) ---
var auth = app.MapGroup("/api/v1/auth");

auth.MapPost("/login", async (LoginRequest req, AccesoService acceso, CancellationToken ct) =>
{
    var r = await acceso.IniciarSesionAsync(req.NombreUsuario, req.Clave, ct);
    return r.EsExito ? Results.Ok(r.Valor) : MapeoErrores.AProblema(r.Codigo);
});

auth.MapPost("/reingreso", async (ReingresoRequest req, AccesoService acceso, CancellationToken ct) =>
{
    var r = await acceso.ReingresoAsync(req.NombreUsuario, req.MetodoSeguridadPresente, ct);
    return r.EsExito ? Results.Ok(r.Valor) : MapeoErrores.AProblema(r.Codigo);
});

auth.MapPost("/refresh", async (RefreshRequest req, ClaimsPrincipal usuario, AccesoService acceso, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var id))
    {
        return Results.Unauthorized();
    }

    var r = await acceso.RefrescarTokenAsync(id, req.MetodoSeguridadPresente, ct);
    return r.EsExito ? Results.Ok(r.Valor) : MapeoErrores.AProblema(r.Codigo);
}).RequireAuthorization();

auth.MapPost("/metodo-seguridad", async (ClaimsPrincipal usuario, AccesoService acceso, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var id))
    {
        return Results.Unauthorized();
    }

    var r = await acceso.ConfigurarMetodoSeguridadAsync(id, ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
}).RequireAuthorization();

auth.MapPost("/offline", async (ClaimsPrincipal usuario, AccesoService acceso, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var id))
    {
        return Results.Unauthorized();
    }

    var r = await acceso.HabilitarOfflineAsync(id, ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
}).RequireAuthorization();

// --- Gestión de usuarios (CU-03; US-01, US-02, US-31) ---
var usuarios = app.MapGroup("/api/v1/usuarios").RequireAuthorization();

usuarios.MapPost("/", async (AltaUsuarioRequest req, ClaimsPrincipal admin, GestionUsuariosService gestion, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(admin, out var adminId))
    {
        return Results.Unauthorized();
    }

    var r = await gestion.AltaUsuarioAsync(adminId, req.Nombre, (RolJerarquico)req.Rol, req.AreaId, ct);
    return r.EsExito ? Results.Created($"/api/v1/usuarios/{r.Valor!.UsuarioId}", AMapa(r.Valor!)) : MapeoErrores.AProblema(r.Codigo);
});

usuarios.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal admin, GestionUsuariosService gestion, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(admin, out var adminId))
    {
        return Results.Unauthorized();
    }

    var r = await gestion.BajaUsuarioAsync(adminId, id, ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

usuarios.MapPut("/{id:guid}/area", async (Guid id, AsociarAreaRequest req, ClaimsPrincipal admin, GestionUsuariosService gestion, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(admin, out var adminId))
    {
        return Results.Unauthorized();
    }

    var r = await gestion.AsociarAreaAsync(adminId, id, req.AreaId, ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

usuarios.MapGet("/", async (ClaimsPrincipal solicitante, GestionUsuariosService gestion, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(solicitante, out var id))
    {
        return Results.Unauthorized();
    }

    var lista = await gestion.ListarVisiblesAsync(id, ct);
    return Results.Ok(lista.Select(AMapa));
});

// --- Relevamientos (CU-01, CU-10; US-06/07/08/09/10) — módulo CQRS ligero ---
var relevamientos = app.MapGroup("/api/v1/relevamientos").RequireAuthorization();

relevamientos.MapPost("/", async (CrearRelevamientoRequest req, ClaimsPrincipal jefe, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(jefe, out var jefeId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new CrearRelevamientoCommand(jefeId, req.IdentificacionObra, req.RadioAgrupacionMetros), ct);
    return r.EsExito
        ? Results.Created($"/api/v1/relevamientos/{r.Valor!.RelevamientoId}", AMapaRelevamiento(r.Valor!))
        : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapPost("/{id:guid}/agentes", async (Guid id, AsignarAgentesRequest req, ClaimsPrincipal jefe, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(jefe, out var jefeId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new AsignarAgentesCommand(jefeId, id, req.AgentesIds), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapPut("/{id:guid}/agentes", async (Guid id, AsignarAgentesRequest req, ClaimsPrincipal jefe, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(jefe, out var jefeId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new ReasignarAgentesCommand(jefeId, id, req.AgentesIds), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapPost("/{id:guid}/transicion", async (Guid id, TransicionRequest req, ClaimsPrincipal jefe, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(jefe, out var jefeId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new TransicionarEstadoCommand(jefeId, id, (EstadoRelevamiento)req.EstadoDestino), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapPost("/{id:guid}/reabrir", async (Guid id, ClaimsPrincipal jefe, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(jefe, out var jefeId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new ReabrirRelevamientoCommand(jefeId, id), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapGet("/", async (ClaimsPrincipal solicitante, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(solicitante, out var id))
    {
        return Results.Unauthorized();
    }

    var lista = await mediador.EnviarAsync(new ListarRelevamientosQuery(id), ct);
    return Results.Ok(lista.Select(AMapaRelevamiento));
});

// --- Captura y georreferenciación (CU-04, CU-05; US-11/12/13/14) ---
relevamientos.MapPost("/{relevamientoId:guid}/observaciones", async (Guid relevamientoId, CapturarObservacionRequest req, ClaimsPrincipal agente, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(agente, out var agenteId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(
        new CapturarObservacionCommand(agenteId, relevamientoId, req.ReferenciaArchivo, req.LatitudExif, req.LongitudExif), ct);
    if (!r.EsExito)
    {
        return MapeoErrores.AProblema(r.Codigo);
    }

    var capt = r.Valor!;
    return Results.Created(
        $"/api/v1/relevamientos/{relevamientoId}/observaciones/{capt.ObservacionId}",
        new CapturaResponse(capt.ObservacionId, capt.MarcadorId, capt.SinGeorreferenciar));
});

relevamientos.MapGet("/{relevamientoId:guid}/observaciones", async (Guid relevamientoId, ClaimsPrincipal solicitante, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(solicitante, out var id))
    {
        return Results.Unauthorized();
    }

    var lista = await mediador.EnviarAsync(new ListarObservacionesQuery(id, relevamientoId), ct);
    return Results.Ok(lista.Select(AMapaObservacion));
});

var observaciones = app.MapGroup("/api/v1/observaciones").RequireAuthorization();

observaciones.MapPost("/{observacionId:guid}/ubicacion", async (Guid observacionId, UbicarManualRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new UbicarObservacionManualCommand(usuarioId, observacionId, req.Latitud, req.Longitud), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

// --- Provisión de credenciales (BT-23) ---
usuarios.MapPost("/{id:guid}/credencial", async (Guid id, EstablecerCredencialRequest req, ClaimsPrincipal admin, ProvisionCredencialService provision, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(admin, out var adminId))
    {
        return Results.Unauthorized();
    }

    var r = await provision.EstablecerCredencialAsync(adminId, id, req.NombreUsuario, req.Clave, ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

// --- Revisión sobre mapa y gestión de marcador (CU-08, CU-09; US-15/21/22) ---
relevamientos.MapGet("/{relevamientoId:guid}/revision", async (Guid relevamientoId, string? etiquetas, ClaimsPrincipal solicitante, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(solicitante, out var id))
    {
        return Results.Unauthorized();
    }

    var filtro = string.IsNullOrWhiteSpace(etiquetas)
        ? Array.Empty<string>()
        : etiquetas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var revision = await mediador.EnviarAsync(new RevisarRelevamientoQuery(id, relevamientoId, filtro), ct);
    return revision is null ? Results.NotFound() : Results.Ok(AMapaRevision(revision));
});

var marcadores = app.MapGroup("/api/v1/marcadores").RequireAuthorization();
marcadores.MapPost("/{marcadorId:guid}/comentarios", async (Guid marcadorId, AgregarComentarioRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new AgregarComentarioCommand(usuarioId, marcadorId, req.FotoId, req.Texto), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

var fotos = app.MapGroup("/api/v1/fotos").RequireAuthorization();
fotos.MapPost("/{fotoId:guid}/etiquetas", async (Guid fotoId, EtiquetarRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new EtiquetarFotoCommand(usuarioId, fotoId, req.Etiqueta), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

// Subida del binario de una foto al backend de alojamiento (CU-04, ADR-08; BT-20).
fotos.MapPost("/{fotoId:guid}/contenido", async (Guid fotoId, IFormFile archivo, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    using var memoria = new MemoryStream();
    await archivo.CopyToAsync(memoria, ct);

    var r = await mediador.EnviarAsync(new SubirContenidoFotoCommand(usuarioId, fotoId, archivo.FileName, memoria.ToArray()), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
}).DisableAntiforgery();

// Descarga del binario de una foto para el visor a pantalla completa (CU-09, US-24).
fotos.MapGet("/{fotoId:guid}/contenido", async (Guid fotoId, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var contenido = await mediador.EnviarAsync(new DescargarContenidoFotoQuery(usuarioId, fotoId), ct);
    return contenido is null ? Results.NotFound() : Results.File(contenido, "image/jpeg");
});

var comentarios = app.MapGroup("/api/v1/comentarios").RequireAuthorization();
comentarios.MapPost("/{comentarioId:guid}/etiquetas", async (Guid comentarioId, EtiquetarRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new EtiquetarComentarioCommand(usuarioId, comentarioId, req.Etiqueta), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

// --- Detección y resolución de conflictos por radio (CU-11, CU-12; US-25/26) ---
relevamientos.MapPost("/{relevamientoId:guid}/conflictos/deteccion", async (Guid relevamientoId, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new DetectarConflictosCommand(usuarioId, relevamientoId), ct);
    return r.EsExito
        ? Results.Ok(r.Valor!.Select(c => new ConflictoDetectadoDto(c.ConflictoSyncId, c.MarcadorA, c.MarcadorB, c.DistanciaMetros)))
        : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapGet("/{relevamientoId:guid}/conflictos", async (Guid relevamientoId, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var lista = await mediador.EnviarAsync(new ConflictosPendientesQuery(usuarioId, relevamientoId), ct);
    return Results.Ok(lista.Select(c => new ConflictoPendienteDto(c.ConflictoSyncId, c.Tipo, c.MarcadorA, c.MarcadorB)));
});

relevamientos.MapPut("/{relevamientoId:guid}/radio", async (Guid relevamientoId, AjustarRadioRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new AjustarRadioCommand(usuarioId, relevamientoId, req.RadioMetros), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

var conflictos = app.MapGroup("/api/v1/conflictos").RequireAuthorization();
conflictos.MapPost("/{conflictoId:guid}/resolucion", async (Guid conflictoId, ResolverConflictoRequest req, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(
        new ResolverConflictoCommand(usuarioId, conflictoId, (DecisionConflicto)req.Decision, req.MarcadorResultanteId), ct);
    return r.EsExito ? Results.NoContent() : MapeoErrores.AProblema(r.Codigo);
});

// --- Exportación e importación del relevamiento completo (CU-08 §5.A/§5.B; US-27/US-28) ---
relevamientos.MapGet("/{relevamientoId:guid}/export", async (Guid relevamientoId, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    var r = await mediador.EnviarAsync(new ExportarRelevamientoCommand(usuarioId, relevamientoId), ct);
    return r.EsExito
        ? Results.File(r.Valor!.Contenido, "application/zip", r.Valor!.NombreArchivo)
        : MapeoErrores.AProblema(r.Codigo);
});

relevamientos.MapPost("/import", async (IFormFile archivo, ClaimsPrincipal usuario, IMediador mediador, CancellationToken ct) =>
{
    if (!TryGetUsuarioId(usuario, out var usuarioId))
    {
        return Results.Unauthorized();
    }

    using var memoria = new MemoryStream();
    await archivo.CopyToAsync(memoria, ct);

    var r = await mediador.EnviarAsync(new ImportarRelevamientoCommand(usuarioId, memoria.ToArray()), ct);
    return r.EsExito
        ? Results.Created($"/api/v1/relevamientos/{r.Valor}", new { relevamientoId = r.Valor })
        : MapeoErrores.AProblema(r.Codigo);
}).DisableAntiforgery();

app.Run();

static bool TryGetUsuarioId(ClaimsPrincipal principal, out Guid id)
{
    var sub = principal.FindFirst("sub")?.Value;
    return Guid.TryParse(sub, out id);
}

static UsuarioDto AMapa(Usuario u) =>
    new(u.UsuarioId, u.Nombre, (int)u.Rol, u.AreaId, u.EstadoVigencia, u.MetodoSeguridadConfigurado);

static RelevamientoDto AMapaRelevamiento(Relevamiento r) =>
    new(r.RelevamientoId, r.IdentificacionObra, (int)r.Estado, r.RadioAgrupacionMetros, r.AreaId, r.AgentesVigentes());

static ObservacionDto AMapaObservacion(Observacion o) =>
    new(o.ObservacionId, o.RelevamientoId, o.MarcadorId, o.AgenteUsuarioId, o.SinGeorreferenciar);

static RevisionRelevamientoDto AMapaRevision(RevisionRelevamiento r) =>
    new(
        r.RelevamientoId,
        r.Estado,
        r.Marcadores.Select(m => new RevisionMarcadorDto(
            m.MarcadorId, m.Latitud, m.Longitud, m.EnConflicto,
            m.Fotos.Select(f => new RevisionFotoDto(f.FotoId, f.ReferenciaArchivo, f.Etiquetas)).ToList(),
            m.Comentarios.Select(c => new RevisionComentarioDto(c.ComentarioId, c.Texto, c.FotoId, c.Etiquetas)).ToList())).ToList(),
        r.ObservacionesSinGeorreferenciar);

/// <summary>Punto de entrada expuesto para pruebas de integración (WebApplicationFactory).</summary>
public partial class Program;

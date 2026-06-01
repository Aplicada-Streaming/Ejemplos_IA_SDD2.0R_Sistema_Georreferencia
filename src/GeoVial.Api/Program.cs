using System.Security.Claims;
using System.Text;
using GeoVial.Api;
using GeoVial.Application;
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

app.Run();

static bool TryGetUsuarioId(ClaimsPrincipal principal, out Guid id)
{
    var sub = principal.FindFirst("sub")?.Value;
    return Guid.TryParse(sub, out id);
}

static UsuarioDto AMapa(Usuario u) =>
    new(u.UsuarioId, u.Nombre, (int)u.Rol, u.AreaId, u.EstadoVigencia, u.MetodoSeguridadConfigurado);

/// <summary>Punto de entrada expuesto para pruebas de integración (WebApplicationFactory).</summary>
public partial class Program;

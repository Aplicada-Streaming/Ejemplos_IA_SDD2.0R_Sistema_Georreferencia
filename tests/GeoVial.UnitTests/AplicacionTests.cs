using FluentAssertions;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Gestión de usuarios (CU-03; US-01, US-02; RN-07).</summary>
public class GestionUsuariosServiceTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();

    [Fact] // CU-03 CA-01 + RN-07: alta exitosa y auditada
    public async Task Alta_jefe_de_area_por_jefe_general_exito_y_audita()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var auditoria = new FakeAuditoria();
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin), new FakeAreaRepository(AreaNorte), auditoria);

        var r = await svc.AltaUsuarioAsync(admin.UsuarioId, "Jefe Norte", RolJerarquico.JefeArea, AreaNorte);

        r.EsExito.Should().BeTrue();
        r.Valor!.Rol.Should().Be(RolJerarquico.JefeArea);
        r.Valor!.AreaId.Should().Be(AreaNorte);
        auditoria.Registros.Should().Contain(x => x.StartsWith("ALTA_USUARIO:"));
    }

    [Fact] // CU-03 CA-02: un jefe de área no puede dar de alta otro jefe de área
    public async Task Alta_por_jefe_de_area_de_otro_jefe_rechaza()
    {
        var admin = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.AltaUsuarioAsync(admin.UsuarioId, "Otro", RolJerarquico.JefeArea, AreaNorte);

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    // CU-03 CA-03 (validación de área inexistente). Nota de implementación: CA-03 cita "agente" como
    // objetivo de un jefe general, pero el nivel inmediato inferior del jefe general es el jefe de área
    // (§3, BT-02). Se sigue la invariante rectora y se ejercita AREA_INEXISTENTE con el rol correcto.
    [Fact]
    public async Task Alta_con_area_inexistente_rechaza()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var areaInexistente = Guid.NewGuid();
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin), new FakeAreaRepository(), new FakeAuditoria());

        var r = await svc.AltaUsuarioAsync(admin.UsuarioId, "Jefe X", RolJerarquico.JefeArea, areaInexistente);

        r.Codigo.Should().Be(CodigosError.AreaInexistente);
    }

    [Fact] // RN-07 / CU-03: si no se puede auditar, se rechaza con ACCION_NO_AUDITADA
    public async Task Alta_sin_auditoria_rechaza()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin), new FakeAreaRepository(AreaNorte), new FakeAuditoria(exito: false));

        var r = await svc.AltaUsuarioAsync(admin.UsuarioId, "Jefe Norte", RolJerarquico.JefeArea, AreaNorte);

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    [Fact] // CU-03 §5.A: baja jerárquica válida
    public async Task Baja_de_jefe_de_area_por_jefe_general_exito()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.BajaUsuarioAsync(admin.UsuarioId, objetivo.UsuarioId);

        r.EsExito.Should().BeTrue();
        objetivo.EstadoVigencia.Should().BeFalse();
    }

    [Fact] // US-31 / RN-01: el listado solo devuelve usuarios del ámbito del solicitante
    public async Task Listar_visibles_filtra_por_area_del_jefe()
    {
        var jefeNorte = Usuario.Crear("jn", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var agenteNorte = Usuario.Crear("an", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var agenteSur = Usuario.Crear("as", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var svc = new GestionUsuariosService(
            new FakeUsuarioRepository(jefeNorte, agenteNorte, agenteSur), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var visibles = await svc.ListarVisiblesAsync(jefeNorte.UsuarioId);

        visibles.Select(u => u.UsuarioId).Should().Contain(agenteNorte.UsuarioId).And.NotContain(agenteSur.UsuarioId);
    }
}

/// <summary>Acceso, login, relogueo y refresh (CU-02; US-04, US-05; RN-06; ADR-03).</summary>
public class AccesoServiceTests
{
    private static AccesoService Crear(Usuario usuario, Credencial? cred)
    {
        var creds = cred is null ? new FakeCredencialRepository() : new FakeCredencialRepository(cred);
        return new AccesoService(creds, new FakeUsuarioRepository(usuario), new FakeHasher(), new FakeToken(), new FakeAuditoria());
    }

    private static Credencial CredDe(Usuario u, string clave) => new(u.UsuarioId, "agente", new FakeHasher().Hash(clave));

    [Fact] // CU-02: login válido → token
    public async Task Login_valido_devuelve_token()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var r = await Crear(u, CredDe(u, "secreta")).IniciarSesionAsync("agente", "secreta");

        r.EsExito.Should().BeTrue();
        r.Valor!.AccessToken.Should().Be($"access-{u.UsuarioId}");
        r.Valor!.ExpiraEnSegundos.Should().Be(3600);
    }

    [Fact] // CU-02: credenciales inválidas
    public async Task Login_invalido_devuelve_credenciales_invalidas()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var r = await Crear(u, CredDe(u, "secreta")).IniciarSesionAsync("agente", "mala");

        r.Codigo.Should().Be(CodigosError.CredencialesInvalidas);
    }

    [Fact] // CU-02 CA-02 / RN-06: habilitar offline sin método configurado
    public async Task Habilitar_offline_sin_metodo_falla()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var r = await Crear(u, null).HabilitarOfflineAsync(u.UsuarioId);

        r.Codigo.Should().Be(CodigosError.OfflineNoHabilitado);
    }

    [Fact] // RN-06: con método configurado habilita el modo sin conexión
    public async Task Habilitar_offline_con_metodo_exito()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        u.ConfigurarMetodoSeguridad();

        (await Crear(u, null).HabilitarOfflineAsync(u.UsuarioId)).EsExito.Should().BeTrue();
    }

    [Fact] // CU-02 CA-03 / US-05: reingreso sin método de seguridad
    public async Task Reingreso_sin_metodo_falla()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var r = await Crear(u, CredDe(u, "x")).ReingresoAsync("agente", metodoSeguridadPresente: false);

        r.Codigo.Should().Be(CodigosError.ReingresoSinMetodoSeguridad);
    }

    [Fact] // US-05: reingreso con método presente y configurado → token
    public async Task Reingreso_con_metodo_devuelve_token()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        u.ConfigurarMetodoSeguridad();
        var r = await Crear(u, CredDe(u, "x")).ReingresoAsync("agente", metodoSeguridadPresente: true);

        r.EsExito.Should().BeTrue();
    }

    [Fact] // ADR-03: el refresh del agente se condiciona al método de seguridad del teléfono
    public async Task Refresh_agente_sin_metodo_falla()
    {
        var u = Usuario.Crear("agente", RolJerarquico.AgenteCampo, Guid.NewGuid()).Valor!;
        var r = await Crear(u, null).RefrescarTokenAsync(u.UsuarioId, metodoSeguridadPresente: false);

        r.Codigo.Should().Be(CodigosError.ReingresoSinMetodoSeguridad);
    }
}

/// <summary>Autorización transversal (CU-14, US-31, RN-01/RN-08).</summary>
public class AutorizacionServiceTests
{
    [Fact] // CU-14 CA-01 + RN-08: acceso a otra área se rechaza y se audita
    public async Task Acceso_a_otra_area_rechaza_y_audita()
    {
        var areaNorte = Guid.NewGuid();
        var areaSur = Guid.NewGuid();
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, areaNorte).Valor!;
        var aud = new FakeAuditoria();
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefe), aud);

        var r = await svc.EvaluarAccesoAreaAsync(jefe.UsuarioId, areaSur);

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
        aud.Registros.Should().Contain(x => x.StartsWith("ACCESO_RECHAZADO:"));
    }

    [Fact] // CU-14 CA-03: acceso a su propia área se concede
    public async Task Acceso_a_su_area_exito()
    {
        var areaNorte = Guid.NewGuid();
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, areaNorte).Valor!;
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefe), new FakeAuditoria());

        (await svc.EvaluarAccesoAreaAsync(jefe.UsuarioId, areaNorte)).EsExito.Should().BeTrue();
    }

    [Fact] // CU-14 CA-02: acceso a datos personales de otro agente se rechaza
    public async Task Acceso_dato_personal_de_otro_agente_rechaza()
    {
        var area = Guid.NewGuid();
        var a1 = Usuario.Crear("a1", RolJerarquico.AgenteCampo, area).Valor!;
        var a2 = Usuario.Crear("a2", RolJerarquico.AgenteCampo, area).Valor!;
        var svc = new AutorizacionService(new FakeUsuarioRepository(a1, a2), new FakeAuditoria());

        var r = await svc.EvaluarAccesoDatoPersonalAsync(a1.UsuarioId, a2.UsuarioId);

        r.Codigo.Should().Be(CodigosError.AccesoDatoPersonalNoAutorizado);
    }
}

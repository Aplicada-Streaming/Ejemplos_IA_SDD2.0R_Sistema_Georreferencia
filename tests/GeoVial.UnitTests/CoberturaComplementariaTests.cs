using FluentAssertions;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Tests complementarios que cubren caminos de borde de dominio y aplicación para alcanzar el gate
/// de cobertura de la DoD (líneas ≥ 80%, branches ≥ 70% en dominio y aplicación).
/// </summary>
public class DominioBordeTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    [Fact]
    public void Area_crear_valida_nombre()
    {
        Area.Crear("Zona Norte").Nombre.Should().Be("Zona Norte");
        var accion = () => Area.Crear("  ");
        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Area_asignar_jefe_guarda_referencia()
    {
        var area = Area.Crear("Zona Norte");
        var jefe = Guid.NewGuid();
        area.AsignarJefe(jefe);
        area.JefeAreaUsuarioId.Should().Be(jefe);
    }

    [Fact] // RC-05: el agente también requiere área
    public void Usuario_agente_sin_area_falla() =>
        Usuario.Crear("a", RolJerarquico.AgenteCampo, null).Codigo.Should().Be(CodigosError.AreaRequerida);

    [Fact]
    public void Usuario_configurar_metodo_y_asociar_area()
    {
        var u = Usuario.Crear("a", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        u.MetodoSeguridadConfigurado.Should().BeFalse();
        u.ConfigurarMetodoSeguridad();
        u.MetodoSeguridadConfigurado.Should().BeTrue();
        u.AsociarArea(AreaSur);
        u.AreaId.Should().Be(AreaSur);
    }

    [Fact]
    public void Jerarquia_nivel_inmediato_inferior()
    {
        Jerarquia.NivelInmediatoInferior(RolJerarquico.Raiz).Should().Be(RolJerarquico.JefeGeneral);
        Jerarquia.NivelInmediatoInferior(RolJerarquico.JefeGeneral).Should().Be(RolJerarquico.JefeArea);
        Jerarquia.NivelInmediatoInferior(RolJerarquico.JefeArea).Should().Be(RolJerarquico.AgenteCampo);
        Jerarquia.NivelInmediatoInferior(RolJerarquico.AgenteCampo).Should().BeNull();
    }

    [Fact] // un administrador no vigente no administra
    public void Jerarquia_administrador_no_vigente_no_administra()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        admin.DarDeBaja();
        Jerarquia.PuedeAdministrar(admin, RolJerarquico.JefeArea, AreaNorte).Should().BeFalse();
    }

    [Fact] // raíz no administra dos niveles abajo (jefe de área no es su inmediato inferior)
    public void Raiz_no_administra_jefe_de_area()
    {
        var raiz = Usuario.Crear("r", RolJerarquico.Raiz, null).Valor!;
        Jerarquia.PuedeAdministrar(raiz, RolJerarquico.JefeArea, AreaNorte).Should().BeFalse();
    }

    [Fact]
    public void Autorizacion_agente_accede_a_su_area_y_no_a_otra()
    {
        var agente = Usuario.Crear("a", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        Autorizacion.PuedeAccederArea(agente, AreaNorte).Should().BeTrue();
        Autorizacion.PuedeAccederArea(agente, AreaSur).Should().BeFalse();
    }

    [Fact]
    public void Autorizacion_usuario_no_vigente_no_accede()
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        jefe.DarDeBaja();
        Autorizacion.PuedeAccederArea(jefe, AreaNorte).Should().BeFalse();
    }

    [Fact]
    public void Autorizacion_dato_personal_por_rol()
    {
        var general = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var jefeNorte = Usuario.Crear("jn", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var agenteNorte = Usuario.Crear("an", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var agenteSur = Usuario.Crear("as", RolJerarquico.AgenteCampo, AreaSur).Valor!;

        Autorizacion.PuedeAccederDatoPersonal(general, agenteSur).Should().BeTrue();
        Autorizacion.PuedeAccederDatoPersonal(jefeNorte, agenteNorte).Should().BeTrue();
        Autorizacion.PuedeAccederDatoPersonal(jefeNorte, agenteSur).Should().BeFalse();
    }

    [Fact] // RN-07: el asiento de auditoría captura autor, momento, operación y recurso
    public void RegistroAuditoria_captura_los_datos()
    {
        var autor = Guid.NewGuid();
        var momento = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var registro = new RegistroAuditoria(autor, momento, "ALTA_USUARIO", "usuario=123");

        registro.RegistroAuditoriaId.Should().NotBe(Guid.Empty);
        registro.AutorUsuarioId.Should().Be(autor);
        registro.Momento.Should().Be(momento);
        registro.Operacion.Should().Be("ALTA_USUARIO");
        registro.RecursoAfectado.Should().Be("usuario=123");
    }

    [Fact] // la credencial liga un nombre de usuario a un hash de clave (soporte ROPC)
    public void Credencial_liga_usuario_y_hash()
    {
        var usuarioId = Guid.NewGuid();
        var cred = new Credencial(usuarioId, "raiz", "h:clave");

        cred.CredencialId.Should().NotBe(Guid.Empty);
        cred.UsuarioId.Should().Be(usuarioId);
        cred.NombreUsuario.Should().Be("raiz");
        cred.HashClave.Should().Be("h:clave");
    }

    [Fact]
    public void Resultado_exito_y_fallo()
    {
        Resultado.Exito().EsExito.Should().BeTrue();
        Resultado.Fallo("X").Codigo.Should().Be("X");
        Resultado<int>.Exito(7).Valor.Should().Be(7);
        Resultado<int>.Fallo("Y").EsExito.Should().BeFalse();
    }
}

public class AplicacionBordeTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();

    [Fact] // alta con usuario administrador inexistente
    public async Task Alta_con_admin_inexistente_falla()
    {
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(), new FakeAreaRepository(AreaNorte), new FakeAuditoria());
        var r = await svc.AltaUsuarioAsync(Guid.NewGuid(), "x", RolJerarquico.JefeArea, AreaNorte);
        r.Codigo.Should().Be(CodigosError.UsuarioInexistente);
    }

    [Fact] // asociar área: éxito
    public async Task Asociar_area_exito()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.AsociarAreaAsync(admin.UsuarioId, objetivo.UsuarioId, AreaNorte);

        r.EsExito.Should().BeTrue();
        objetivo.AreaId.Should().Be(AreaNorte);
    }

    [Fact] // asociar área inexistente
    public async Task Asociar_area_inexistente_falla()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(), new FakeAuditoria());

        var r = await svc.AsociarAreaAsync(admin.UsuarioId, objetivo.UsuarioId, Guid.NewGuid());

        r.Codigo.Should().Be(CodigosError.AreaInexistente);
    }

    [Fact] // asociar área sin autorización (jefe de área no administra otro jefe de área)
    public async Task Asociar_area_no_autorizado_falla()
    {
        var admin = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var objetivo = Usuario.Crear("otro", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.AsociarAreaAsync(admin.UsuarioId, objetivo.UsuarioId, AreaNorte);

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // baja con objetivo inexistente
    public async Task Baja_con_objetivo_inexistente_falla()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.BajaUsuarioAsync(admin.UsuarioId, Guid.NewGuid());

        r.Codigo.Should().Be(CodigosError.UsuarioInexistente);
    }

    [Fact] // baja sin autorización
    public async Task Baja_no_autorizada_falla()
    {
        var admin = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var objetivo = Usuario.Crear("otro", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(AreaNorte), new FakeAuditoria());

        var r = await svc.BajaUsuarioAsync(admin.UsuarioId, objetivo.UsuarioId);

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // baja sin auditoría → ACCION_NO_AUDITADA
    public async Task Baja_sin_auditoria_falla()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(admin, objetivo), new FakeAreaRepository(AreaNorte), new FakeAuditoria(exito: false));

        var r = await svc.BajaUsuarioAsync(admin.UsuarioId, objetivo.UsuarioId);

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    [Fact] // listar visibles con solicitante inexistente devuelve vacío
    public async Task Listar_visibles_solicitante_inexistente_vacio()
    {
        var svc = new GestionUsuariosService(new FakeUsuarioRepository(), new FakeAreaRepository(), new FakeAuditoria());
        (await svc.ListarVisiblesAsync(Guid.NewGuid())).Should().BeEmpty();
    }

    [Fact] // configurar método de seguridad: éxito e inexistente
    public async Task Configurar_metodo_seguridad()
    {
        var u = Usuario.Crear("a", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var acceso = new AccesoService(new FakeCredencialRepository(), new FakeUsuarioRepository(u), new FakeHasher(), new FakeToken(), new FakeAuditoria());

        (await acceso.ConfigurarMetodoSeguridadAsync(u.UsuarioId)).EsExito.Should().BeTrue();
        u.MetodoSeguridadConfigurado.Should().BeTrue();
        (await acceso.ConfigurarMetodoSeguridadAsync(Guid.NewGuid())).Codigo.Should().Be(CodigosError.UsuarioInexistente);
    }

    [Fact] // refresh de un jefe (no agente) no exige método de seguridad
    public async Task Refresh_no_agente_exito()
    {
        var u = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var acceso = new AccesoService(new FakeCredencialRepository(), new FakeUsuarioRepository(u), new FakeHasher(), new FakeToken(), new FakeAuditoria());

        (await acceso.RefrescarTokenAsync(u.UsuarioId, metodoSeguridadPresente: false)).EsExito.Should().BeTrue();
    }

    [Fact] // reingreso con usuario sin credencial → credenciales inválidas
    public async Task Reingreso_sin_credencial_falla()
    {
        var u = Usuario.Crear("a", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var acceso = new AccesoService(new FakeCredencialRepository(), new FakeUsuarioRepository(u), new FakeHasher(), new FakeToken(), new FakeAuditoria());

        (await acceso.ReingresoAsync("inexistente", metodoSeguridadPresente: true)).Codigo.Should().Be(CodigosError.CredencialesInvalidas);
    }

    [Fact] // evaluar acceso a dato personal: éxito y usuario inexistente
    public async Task Evaluar_dato_personal()
    {
        var jefe = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var agente = Usuario.Crear("a", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefe, agente), new FakeAuditoria());

        (await svc.EvaluarAccesoDatoPersonalAsync(jefe.UsuarioId, agente.UsuarioId)).EsExito.Should().BeTrue();
        (await svc.EvaluarAccesoAreaAsync(Guid.NewGuid(), AreaNorte)).Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }
}

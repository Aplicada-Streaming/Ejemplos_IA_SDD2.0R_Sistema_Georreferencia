using FluentAssertions;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Finalidad de tratamiento de datos personales (RN-08, Ley 25.326).</summary>
public class FinalidadesTests
{
    [Theory] // RN-08: solo los fines del relevamiento y su evaluación están permitidos
    [InlineData("Relevamiento")]
    [InlineData("relevamiento")]
    [InlineData("Evaluacion")]
    public void Finalidad_permitida(string finalidad) => Finalidades.EsPermitida(finalidad).Should().BeTrue();

    [Theory] // cualquier otra finalidad se bloquea
    [InlineData("marketing")]
    [InlineData("")]
    [InlineData(null)]
    public void Finalidad_no_permitida(string? finalidad) => Finalidades.EsPermitida(finalidad).Should().BeFalse();
}

/// <summary>Inmutabilidad del registro de auditoría (RN-07): sin mutadores públicos.</summary>
public class RegistroAuditoriaInmutableTests
{
    [Fact] // RN-07 / CU-13 CA-02: el registro no expone forma de alterarlo
    public void RegistroAuditoria_no_tiene_setters_publicos()
    {
        var propiedadesEscribibles = typeof(RegistroAuditoria).GetProperties()
            .Where(p => p.SetMethod is { IsPublic: true })
            .Select(p => p.Name);

        propiedadesEscribibles.Should().BeEmpty();
        typeof(RegistroAuditoria).GetMethods()
            .Where(m => m.IsPublic && !m.IsStatic && (m.Name.StartsWith("Set") || m.Name is "Editar" or "Eliminar" or "Modificar"))
            .Should().BeEmpty();
    }
}

/// <summary>Consulta del historial de auditoría con retención (US-30, CU-13).</summary>
public class ConsultaAuditoriaTests
{
    private static readonly DateTime Ahora = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Usuario Raiz() => Usuario.Crear("raiz", RolJerarquico.Raiz, null).Valor!;

    private static RegistroAuditoria Registro(Guid autor, string operacion, string recurso, DateTime momento) =>
        new(autor, momento, operacion, recurso);

    [Fact] // US-30 CA-01: el raíz consulta y obtiene los registros dentro del rango
    public async Task Raiz_consulta_devuelve_registros()
    {
        var raiz = Raiz();
        var reg = Registro(Guid.NewGuid(), "ALTA_USUARIO", "usuario=x", Ahora.AddDays(-30));
        var svc = new ConsultaAuditoriaService(new FakeUsuarioRepository(raiz), new FakeConsultaAuditoria(reg), new FakeAuditoria(), new FakeReloj());

        var r = await svc.ConsultarAsync(raiz.UsuarioId, null, null, null, null);

        r.EsExito.Should().BeTrue();
        r.Valor.Should().ContainSingle();
    }

    [Fact] // US-30 CA-02: un usuario sin rol raíz es bloqueado y el intento se registra
    public async Task No_raiz_es_bloqueado_y_se_registra()
    {
        var jefe = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var auditoria = new FakeAuditoria();
        var svc = new ConsultaAuditoriaService(new FakeUsuarioRepository(jefe), new FakeConsultaAuditoria(), auditoria, new FakeReloj());

        var r = await svc.ConsultarAsync(jefe.UsuarioId, null, null, null, null);

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
        auditoria.Registros.Should().Contain(x => x.StartsWith("CONSULTA_AUDITORIA_RECHAZADA:"));
    }

    [Fact] // RN-07: por defecto la consulta acota al período de retención (12 meses)
    public async Task Por_defecto_acota_a_doce_meses()
    {
        var raiz = Raiz();
        var dentro = Registro(Guid.NewGuid(), "ACCESO", "login", Ahora.AddMonths(-6));
        var fuera = Registro(Guid.NewGuid(), "ACCESO", "login", Ahora.AddMonths(-13));
        var svc = new ConsultaAuditoriaService(new FakeUsuarioRepository(raiz), new FakeConsultaAuditoria(dentro, fuera), new FakeAuditoria(), new FakeReloj());

        var r = await svc.ConsultarAsync(raiz.UsuarioId, null, null, null, null);

        r.Valor.Should().ContainSingle(); // solo el de hace 6 meses
    }

    [Fact] // filtra por recurso
    public async Task Filtra_por_recurso()
    {
        var raiz = Raiz();
        var login = Registro(Guid.NewGuid(), "ACCESO", "login", Ahora.AddDays(-1));
        var alta = Registro(Guid.NewGuid(), "ALTA_USUARIO", "usuario=x", Ahora.AddDays(-1));
        var svc = new ConsultaAuditoriaService(new FakeUsuarioRepository(raiz), new FakeConsultaAuditoria(login, alta), new FakeAuditoria(), new FakeReloj());

        var r = await svc.ConsultarAsync(raiz.UsuarioId, null, "usuario=", null, null);

        r.Valor.Should().ContainSingle().Which.Operacion.Should().Be("ALTA_USUARIO");
    }
}

/// <summary>Acceso a datos personales con limitación de finalidad (US-31 §5.B, CU-14, RN-08).</summary>
public class AccesoDatosPersonalesTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Usuario Jefe(Guid area) => Usuario.Crear("ja", RolJerarquico.JefeArea, area).Valor!;
    private static Usuario Agente(Guid area) => Usuario.Crear("agente", RolJerarquico.AgenteCampo, area).Valor!;

    [Fact] // CU-14 §5.B: el jefe de área accede a un dato personal de su área con finalidad válida y se audita
    public async Task Acceso_con_finalidad_valida_devuelve_datos_y_audita()
    {
        var jefe = Jefe(AreaNorte);
        var agente = Agente(AreaNorte);
        var auditoria = new FakeAuditoria();
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefe, agente), auditoria);

        var r = await svc.ConsultarDatosPersonalesAsync(jefe.UsuarioId, agente.UsuarioId, "relevamiento");

        r.EsExito.Should().BeTrue();
        r.Valor!.UsuarioId.Should().Be(agente.UsuarioId);
        auditoria.Registros.Should().Contain(x => x.StartsWith("ACCESO_DATO_PERSONAL:"));
    }

    [Fact] // RN-08: una finalidad ajena a la del relevamiento se bloquea
    public async Task Finalidad_no_permitida_se_bloquea()
    {
        var jefe = Jefe(AreaNorte);
        var agente = Agente(AreaNorte);
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefe, agente), new FakeAuditoria());

        var r = await svc.ConsultarDatosPersonalesAsync(jefe.UsuarioId, agente.UsuarioId, "marketing");

        r.Codigo.Should().Be(CodigosError.FinalidadNoPermitida);
    }

    [Fact] // RN-01 / RN-08: un jefe de otra área no accede al dato personal
    public async Task Acceso_fuera_de_area_se_bloquea_y_registra()
    {
        var jefeSur = Jefe(AreaSur);
        var agenteNorte = Agente(AreaNorte);
        var auditoria = new FakeAuditoria();
        var svc = new AutorizacionService(new FakeUsuarioRepository(jefeSur, agenteNorte), auditoria);

        var r = await svc.ConsultarDatosPersonalesAsync(jefeSur.UsuarioId, agenteNorte.UsuarioId, "relevamiento");

        r.Codigo.Should().Be(CodigosError.AccesoDatoPersonalNoAutorizado);
        auditoria.Registros.Should().Contain(x => x.StartsWith("ACCESO_DATO_PERSONAL_RECHAZADO:"));
    }
}

using FluentAssertions;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Reglas de administración jerárquica (BT-02, CU-03 §3, RN-01).</summary>
public class JerarquiaTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Usuario Crear(RolJerarquico rol, Guid? area = null) => Usuario.Crear($"u-{rol}", rol, area).Valor!;

    [Fact] // CU-03 CA-01: el jefe general administra jefes de área
    public void JefeGeneral_administra_jefe_de_area()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.JefeGeneral), RolJerarquico.JefeArea, AreaNorte).Should().BeTrue();
    }

    [Fact] // CU-03 CA-02 / RN-01: un jefe de área no administra otro jefe de área
    public void JefeArea_no_administra_otro_jefe_de_area()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.JefeArea, AreaNorte), RolJerarquico.JefeArea, AreaNorte).Should().BeFalse();
    }

    [Fact] // BT-02: el jefe de área administra agentes de su propia área
    public void JefeArea_administra_agente_de_su_area()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.JefeArea, AreaNorte), RolJerarquico.AgenteCampo, AreaNorte).Should().BeTrue();
    }

    [Fact] // RN-01: el jefe de área no administra agentes de otra área
    public void JefeArea_no_administra_agente_de_otra_area()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.JefeArea, AreaNorte), RolJerarquico.AgenteCampo, AreaSur).Should().BeFalse();
    }

    [Fact] // RN-01: el agente de campo no administra a nadie
    public void Agente_no_administra()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.AgenteCampo, AreaNorte), RolJerarquico.AgenteCampo, AreaNorte).Should().BeFalse();
    }

    [Fact] // CU-03 §3: el usuario raíz administra al jefe general
    public void Raiz_administra_jefe_general()
    {
        Jerarquia.PuedeAdministrar(Crear(RolJerarquico.Raiz), RolJerarquico.JefeGeneral, null).Should().BeTrue();
    }
}

/// <summary>Autorización por rol y área (RN-01, CU-14).</summary>
public class AutorizacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Usuario Crear(RolJerarquico rol, Guid? area = null) => Usuario.Crear($"u-{rol}", rol, area).Valor!;

    [Fact] // CU-14 CA-03
    public void JefeArea_accede_a_su_area() =>
        Autorizacion.PuedeAccederArea(Crear(RolJerarquico.JefeArea, AreaNorte), AreaNorte).Should().BeTrue();

    [Fact] // CU-14 CA-01
    public void JefeArea_no_accede_a_otra_area() =>
        Autorizacion.PuedeAccederArea(Crear(RolJerarquico.JefeArea, AreaNorte), AreaSur).Should().BeFalse();

    [Fact] // RN-01: jefe general accede a cualquier área
    public void JefeGeneral_accede_a_cualquier_area() =>
        Autorizacion.PuedeAccederArea(Crear(RolJerarquico.JefeGeneral), AreaSur).Should().BeTrue();

    [Fact] // CU-14 CA-02 / RN-08
    public void Agente_no_accede_datos_personales_de_otro_agente()
    {
        var a1 = Crear(RolJerarquico.AgenteCampo, AreaNorte);
        var a2 = Crear(RolJerarquico.AgenteCampo, AreaNorte);
        Autorizacion.PuedeAccederDatoPersonal(a1, a2).Should().BeFalse();
    }

    [Fact] // un usuario accede a sus propios datos
    public void Usuario_accede_a_sus_propios_datos()
    {
        var a = Crear(RolJerarquico.AgenteCampo, AreaNorte);
        Autorizacion.PuedeAccederDatoPersonal(a, a).Should().BeTrue();
    }
}

/// <summary>Invariantes de la entidad Usuario (RC-05).</summary>
public class UsuarioTests
{
    [Fact] // RC-05: jefe de área requiere área
    public void JefeArea_sin_area_falla() =>
        Usuario.Crear("x", RolJerarquico.JefeArea, null).Codigo.Should().Be(CodigosError.AreaRequerida);

    [Fact] // nombre obligatorio
    public void Nombre_vacio_falla() =>
        Usuario.Crear("   ", RolJerarquico.JefeGeneral, null).Codigo.Should().Be(CodigosError.NombreRequerido);

    [Fact] // CU-03 §5.A: la baja es lógica
    public void DarDeBaja_marca_no_vigente()
    {
        var u = Usuario.Crear("x", RolJerarquico.JefeGeneral, null).Valor!;
        u.DarDeBaja();
        u.EstadoVigencia.Should().BeFalse();
    }
}

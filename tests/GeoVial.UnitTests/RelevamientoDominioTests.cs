using FluentAssertions;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Reglas del agregado Relevamiento: estados (RN-05), radio (RN-02), asignación (RN-01).</summary>
public class RelevamientoDominioTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Relevamiento Crear(EstadoRelevamiento? estado = null)
    {
        var r = Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        if (estado == EstadoRelevamiento.Revision)
        {
            r.TransicionarA(EstadoRelevamiento.Revision);
        }
        else if (estado == EstadoRelevamiento.Cerrado)
        {
            r.TransicionarA(EstadoRelevamiento.Revision);
            r.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        return r;
    }

    private static Usuario Agente(Guid area) => Usuario.Crear("agente", RolJerarquico.AgenteCampo, area).Valor!;

    [Fact] // CU-01 CA-01: alta en estado recolección con su radio
    public void Crear_en_recoleccion_con_radio()
    {
        var r = Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        r.Estado.Should().Be(EstadoRelevamiento.Recoleccion);
        r.RadioAgrupacionMetros.Should().Be(15m);
        r.EsSoloLectura.Should().BeFalse();
    }

    [Fact] // RN-02: radio no positivo se rechaza
    public void Crear_con_radio_no_positivo_falla() =>
        Relevamiento.Crear("Obra", 0m, AreaNorte).Codigo.Should().Be(CodigosError.RadioInvalido);

    [Fact]
    public void Crear_sin_identificacion_falla() =>
        Relevamiento.Crear("  ", 10m, AreaNorte).Codigo.Should().Be(CodigosError.IdentificacionRequerida);

    [Fact] // CU-10 CA-01: recolección → revisión
    public void Transicion_recoleccion_a_revision_ok()
    {
        var r = Crear();
        r.TransicionarA(EstadoRelevamiento.Revision).EsExito.Should().BeTrue();
        r.Estado.Should().Be(EstadoRelevamiento.Revision);
    }

    [Fact] // revisión → cierre y solo lectura
    public void Transicion_revision_a_cierre_deja_solo_lectura()
    {
        var r = Crear(EstadoRelevamiento.Revision);
        r.TransicionarA(EstadoRelevamiento.Cerrado).EsExito.Should().BeTrue();
        r.EsSoloLectura.Should().BeTrue();
    }

    [Fact] // CU-10 CA-02: recolección → cierre directo es inválido
    public void Transicion_recoleccion_a_cierre_invalida() =>
        Crear().TransicionarA(EstadoRelevamiento.Cerrado).Codigo.Should().Be(CodigosError.TransicionInvalida);

    [Fact] // RN-05: cerrado → recolección por la vía normal exige reapertura explícita
    public void Transicion_cerrado_a_recoleccion_exige_reapertura() =>
        Crear(EstadoRelevamiento.Cerrado).TransicionarA(EstadoRelevamiento.Recoleccion).Codigo.Should().Be(CodigosError.ReaperturaNoAutorizada);

    [Fact] // CU-10 CA-03: reapertura explícita de un cerrado
    public void Reabrir_un_cerrado_vuelve_a_recoleccion()
    {
        var r = Crear(EstadoRelevamiento.Cerrado);
        r.Reabrir().EsExito.Should().BeTrue();
        r.Estado.Should().Be(EstadoRelevamiento.Recoleccion);
        r.EsSoloLectura.Should().BeFalse();
    }

    [Fact] // reabrir un no-cerrado es inválido
    public void Reabrir_un_no_cerrado_falla() =>
        Crear().Reabrir().Codigo.Should().Be(CodigosError.TransicionInvalida);

    [Fact] // CU-01 CA-01: asignar agente del área
    public void Asignar_agente_del_area_ok()
    {
        var r = Crear();
        r.AsignarAgente(Agente(AreaNorte)).EsExito.Should().BeTrue();
        r.AgentesVigentes().Should().HaveCount(1);
    }

    [Fact] // CU-01 CA-02: agente de otra área se rechaza
    public void Asignar_agente_de_otra_area_falla() =>
        Crear().AsignarAgente(Agente(AreaSur)).Codigo.Should().Be(CodigosError.AgenteFueraDeArea);

    [Fact] // un usuario no agente no se asigna
    public void Asignar_no_agente_falla()
    {
        var jefe = Usuario.Crear("jefe", RolJerarquico.JefeArea, AreaNorte).Valor!;
        Crear().AsignarAgente(jefe).Codigo.Should().Be(CodigosError.AgenteFueraDeArea);
    }

    [Fact] // CU-01 CA-03: asignar sobre cerrado se rechaza
    public void Asignar_sobre_cerrado_falla() =>
        Crear(EstadoRelevamiento.Cerrado).AsignarAgente(Agente(AreaNorte)).Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);

    [Fact] // reasignar el mismo agente no duplica
    public void Asignar_dos_veces_no_duplica()
    {
        var r = Crear();
        var agente = Agente(AreaNorte);
        r.AsignarAgente(agente);
        r.AsignarAgente(agente);
        r.AgentesVigentes().Should().HaveCount(1);
    }

    [Fact] // quitar deja la asignación no vigente
    public void Quitar_agente_lo_da_de_baja()
    {
        var r = Crear();
        var agente = Agente(AreaNorte);
        r.AsignarAgente(agente);
        r.QuitarAgente(agente.UsuarioId);
        r.AgentesVigentes().Should().BeEmpty();
    }
}

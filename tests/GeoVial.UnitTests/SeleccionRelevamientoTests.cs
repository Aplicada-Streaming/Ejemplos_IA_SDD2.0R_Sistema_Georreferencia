using System.Text;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Lectura del usuario (claim sub) del JWT, para marcar los relevamientos asignados (F-M-04).</summary>
public class LectorTokenJwtTests
{
    private static string B64Url(string s) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static string Jwt(string payload) => $"{B64Url("{\"alg\":\"HS256\"}")}.{B64Url(payload)}.firma";

    [Fact]
    public void Lee_el_sub_como_guid()
    {
        var id = Guid.NewGuid();
        LectorTokenJwt.LeerUsuarioId(Jwt($"{{\"sub\":\"{id}\",\"role\":4}}")).Should().Be(id);
    }

    [Fact]
    public void Sin_sub_devuelve_null()
    {
        LectorTokenJwt.LeerUsuarioId(Jwt("{\"role\":4}")).Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("no-es-un-jwt")]
    [InlineData("una.sola")]
    public void Token_invalido_devuelve_null(string? token)
    {
        LectorTokenJwt.LeerUsuarioId(token).Should().BeNull();
    }

    [Fact]
    public void Sub_no_guid_devuelve_null()
    {
        LectorTokenJwt.LeerUsuarioId(Jwt("{\"sub\":\"no-guid\"}")).Should().BeNull();
    }
}

/// <summary>Selección de relevamiento del cliente móvil (F-M-04/05): marcar asignados, ordenar y resolver el activo.</summary>
public class SelectorRelevamientosTests
{
    private static RelevamientoDatos Rel(string obra, int estado, params Guid[] agentes) =>
        new(Guid.NewGuid(), obra, estado, agentes);

    [Fact] // F-M-04: marca asignado el del usuario y pone los asignados primero, luego por obra
    public void Listar_marca_asignados_y_ordena()
    {
        var usuario = Guid.NewGuid();
        var zeta = Rel("Zeta", 1);
        var alfa = Rel("Alfa", 1, usuario);
        var beta = Rel("Beta", 1);

        var lista = SelectorRelevamientos.Listar(new[] { zeta, alfa, beta }, usuario);

        lista[0].RelevamientoId.Should().Be(alfa.RelevamientoId); // asignado primero
        lista[0].Asignado.Should().BeTrue();
        lista[1].IdentificacionObra.Should().Be("Beta"); // resto por obra
        lista[2].IdentificacionObra.Should().Be("Zeta");
    }

    [Fact] // sin usuario (no se pudo leer el token) no marca ninguno como asignado
    public void Listar_sin_usuario_no_marca_asignados()
    {
        var lista = SelectorRelevamientos.Listar(new[] { Rel("Obra", 1, Guid.NewGuid()) }, null);
        lista.Should().OnlyContain(r => !r.Asignado);
    }

    [Fact] // F-M-05: si hay selección y sigue disponible, gana
    public void Activo_respeta_la_seleccion()
    {
        var lista = SelectorRelevamientos.Listar(new[] { Rel("A", 1), Rel("B", 1) }, null);
        var elegido = lista[1].RelevamientoId;

        SelectorRelevamientos.Activo(lista, elegido).Should().Be(elegido);
    }

    [Fact] // sin selección, el activo por defecto es el primer asignado abierto
    public void Activo_por_defecto_es_el_primer_asignado_abierto()
    {
        var usuario = Guid.NewGuid();
        var noAsignado = Rel("A", 1);
        var asignado = Rel("B", 1, usuario);
        var lista = SelectorRelevamientos.Listar(new[] { noAsignado, asignado }, usuario);

        SelectorRelevamientos.Activo(lista, null).Should().Be(asignado.RelevamientoId);
    }

    [Fact] // un relevamiento seleccionado que ya no está en la lista no se usa; cae al por defecto
    public void Activo_descarta_seleccion_inexistente()
    {
        var lista = SelectorRelevamientos.Listar(new[] { Rel("A", 1) }, null);
        SelectorRelevamientos.Activo(lista, Guid.NewGuid()).Should().Be(lista[0].RelevamientoId);
    }

    [Fact] // lista vacía → no hay activo
    public void Activo_de_lista_vacia_es_null()
    {
        SelectorRelevamientos.Activo(Array.Empty<RelevamientoResumen>(), null).Should().BeNull();
    }
}

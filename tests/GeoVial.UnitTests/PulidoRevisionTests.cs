using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Caché LRU de fotos del carrusel de revisión (US-21/US-22, pulido móvil).</summary>
public class CacheFotosTests
{
    private static byte[] Bytes(byte b) => new[] { b };

    [Fact] // una clave ausente devuelve null
    public void Clave_ausente_devuelve_null()
    {
        new CacheFotos().Obtener(Guid.NewGuid()).Should().BeNull();
    }

    [Fact] // guardar y recuperar los bytes
    public void Guardar_y_recuperar()
    {
        var cache = new CacheFotos();
        var id = Guid.NewGuid();

        cache.Guardar(id, Bytes(7));

        cache.Obtener(id).Should().Equal(Bytes(7));
    }

    [Fact] // al exceder la capacidad se descarta la entrada menos usada (LRU)
    public void Excede_capacidad_descarta_la_menos_usada()
    {
        var cache = new CacheFotos(capacidad: 2);
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();

        cache.Guardar(a, Bytes(1));
        cache.Guardar(b, Bytes(2));
        cache.Guardar(c, Bytes(3)); // expulsa a la menos usada (a)

        cache.Cantidad.Should().Be(2);
        cache.Obtener(a).Should().BeNull();
        cache.Obtener(b).Should().NotBeNull();
        cache.Obtener(c).Should().NotBeNull();
    }

    [Fact] // acceder a una entrada la vuelve la más reciente y la protege del descarte
    public void Acceder_actualiza_la_recencia()
    {
        var cache = new CacheFotos(capacidad: 2);
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();

        cache.Guardar(a, Bytes(1));
        cache.Guardar(b, Bytes(2));
        cache.Obtener(a);           // a pasa a ser la más reciente
        cache.Guardar(c, Bytes(3)); // ahora se descarta b (la menos usada)

        cache.Obtener(a).Should().NotBeNull();
        cache.Obtener(b).Should().BeNull();
    }

    [Fact] // guardar la misma clave actualiza los bytes sin crecer
    public void Guardar_misma_clave_actualiza()
    {
        var cache = new CacheFotos();
        var id = Guid.NewGuid();

        cache.Guardar(id, Bytes(1));
        cache.Guardar(id, Bytes(9));

        cache.Cantidad.Should().Be(1);
        cache.Obtener(id).Should().Equal(Bytes(9));
    }

    [Fact] // la capacidad debe ser positiva
    public void Capacidad_invalida_falla()
    {
        var crear = () => new CacheFotos(0);
        crear.Should().Throw<ArgumentOutOfRangeException>();
    }
}

/// <summary>Conservar la posición del carrusel al recargar (NavegadorRevision.IrAlMarcador, pulido móvil).</summary>
public class NavegadorIrAlMarcadorTests
{
    private static RevisionMarcadorDto Marcador() =>
        new(Guid.NewGuid(), -34.6m, -58.4m, false, Array.Empty<RevisionFotoDto>(), Array.Empty<RevisionComentarioDto>());

    private static RevisionRelevamientoDto Revision(params RevisionMarcadorDto[] m) =>
        new(Guid.NewGuid(), 1, m, Array.Empty<Guid>());

    [Fact] // IrAlMarcador posiciona el carrusel en el marcador indicado
    public void IrAlMarcador_posiciona_en_el_marcador()
    {
        var m0 = Marcador();
        var m1 = Marcador();
        var m2 = Marcador();
        var nav = new NavegadorRevision(Revision(m0, m1, m2));

        nav.SiguienteFoto(); // (sin fotos, no cambia; solo para asegurar el reinicio luego)
        var ok = nav.IrAlMarcador(m2.MarcadorId);

        ok.Should().BeTrue();
        nav.MarcadorActual.Should().Be(m2);
        nav.IndiceFoto.Should().Be(0);
    }

    [Fact] // un marcador inexistente no cambia la posición
    public void IrAlMarcador_inexistente_no_cambia()
    {
        var m0 = Marcador();
        var m1 = Marcador();
        var nav = new NavegadorRevision(Revision(m0, m1));
        nav.SiguienteMarcador(); // en m1

        var ok = nav.IrAlMarcador(Guid.NewGuid());

        ok.Should().BeFalse();
        nav.MarcadorActual.Should().Be(m1);
    }
}

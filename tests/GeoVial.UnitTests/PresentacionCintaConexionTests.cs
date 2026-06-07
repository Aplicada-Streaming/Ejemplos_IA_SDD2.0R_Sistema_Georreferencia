using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>H-05 (auditoría UX) — la cinta de conexión: ícono + color por estado, sin depender solo del color (WCAG 1.4.1).</summary>
public class PresentacionCintaConexionTests
{
    [Fact] // sin conexión: rojo de alerta con glifo de advertencia y el texto del resumen
    public void Sin_conexion_es_alerta()
    {
        var v = PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(online: false, sincronizando: false, pendientes: 3));

        v.Icono.Should().Be("⚠");
        v.ColorFondo.Should().Be("#b00020");
        v.Texto.Should().Contain("Sin conexión");
    }

    [Fact] // al día: verde con tilde
    public void Al_dia_es_verde()
    {
        var v = PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 0));

        v.Icono.Should().Be("✓");
        v.ColorFondo.Should().Be("#2e7d32");
        v.Texto.Should().Be("Todo sincronizado");
    }

    [Fact] // pendiente: ámbar con texto oscuro (contraste sobre ámbar)
    public void Pendiente_es_ambar_con_texto_oscuro()
    {
        var v = PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 5));

        v.ColorFondo.Should().Be("#e8a000");
        v.ColorTexto.Should().Be("#000000");
    }

    [Fact] // sincronizando: azul
    public void Sincronizando_es_azul()
    {
        var v = PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(online: true, sincronizando: true, pendientes: 5));

        v.Icono.Should().Be("↻");
        v.ColorFondo.Should().Be("#1565c0");
    }

    [Fact] // cada estado tiene un ícono distinto: no se distingue solo por color (WCAG 1.4.1)
    public void Cada_estado_tiene_icono_distinto()
    {
        var iconos = new[]
        {
            PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(true, false, 0)).Icono,
            PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(true, false, 2)).Icono,
            PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(false, false, 2)).Icono,
            PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(true, true, 2)).Icono,
            PresentacionCintaConexion.Para(ResumenSincronizacion.Calcular(true, false, 2, huboError: true)).Icono,
        };

        // al día, pendiente, sin conexión y sincronizando difieren entre sí (error comparte el ⚠ de sin-conexión a propósito).
        iconos[0].Should().NotBe(iconos[1]);
        iconos[1].Should().NotBe(iconos[2]);
        iconos[2].Should().NotBe(iconos[3]);
        iconos[0].Should().NotBe(iconos[2]);
    }
}

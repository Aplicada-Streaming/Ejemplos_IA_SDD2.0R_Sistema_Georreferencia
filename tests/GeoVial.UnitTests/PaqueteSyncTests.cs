using System.IO.Compression;
using FluentAssertions;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Verifica el contenido del paquete NuGet de GeoVial.Sync producido por el build en Release
/// (GeneratePackageOnBuild, EP-09; acción de la retro del Sprint 16). Si el paquete no está presente
/// (por ejemplo, en un build Debug donde no se genera), la prueba se omite.
/// </summary>
public class PaqueteSyncTests
{
    private static string? UbicarNupkg()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "GeoVial.slnx")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            return null;
        }

        var binRelease = Path.Combine(dir.FullName, "src", "GeoVial.Sync", "bin", "Release");
        if (!Directory.Exists(binRelease))
        {
            return null;
        }

        return Directory.GetFiles(binRelease, "GeoVial.Sync.*.nupkg", SearchOption.AllDirectories)
            .FirstOrDefault(f => !f.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase));
    }

    [Fact] // EP-09: el .nupkg incluye la DLL, el README y un nuspec con id y licencia correctos
    public void El_paquete_contiene_dll_readme_y_metadatos()
    {
        var nupkg = UbicarNupkg();
        if (nupkg is null)
        {
            // Build no-Release: el paquete no se genera (GeneratePackageOnBuild es solo Release). Nada que verificar.
            return;
        }

        using var zip = ZipFile.OpenRead(nupkg);
        var entradas = zip.Entries.Select(e => e.FullName).ToList();

        entradas.Should().Contain(e => e == "lib/net10.0/GeoVial.Sync.dll");
        entradas.Should().Contain(e => e == "README.md");

        var nuspecEntry = zip.Entries.Single(e => e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        using var lector = new StreamReader(nuspecEntry.Open());
        var nuspec = lector.ReadToEnd();

        nuspec.Should().Contain("<id>GeoVial.Sync</id>");
        nuspec.Should().Contain("<license type=\"expression\">MIT</license>");
        nuspec.Should().Contain("<readme>README.md</readme>");
    }
}

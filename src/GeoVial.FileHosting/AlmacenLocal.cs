namespace GeoVial.FileHosting;

/// <summary>
/// Backend de alojamiento sobre el sistema de archivos local, en una carpeta raíz configurable
/// (ADR-08). Pensado para desarrollo y despliegues de un solo nodo; para desacoplar el archivo del
/// ciclo de vida del contenedor se usa el backend S3 (R-04 arquitectónico).
/// </summary>
public sealed class AlmacenLocal : IAlmacenFotos
{
    private readonly string _raiz;

    public AlmacenLocal(string rutaRaiz)
    {
        _raiz = rutaRaiz;
        Directory.CreateDirectory(_raiz);
    }

    public async Task<string> GuardarAsync(string nombreSugerido, byte[] contenido, CancellationToken ct = default)
    {
        var referencia = $"{Guid.NewGuid():N}-{Sanear(nombreSugerido)}";
        await File.WriteAllBytesAsync(RutaDe(referencia), contenido, ct);
        return referencia;
    }

    public async Task<byte[]?> RecuperarAsync(string referencia, CancellationToken ct = default)
    {
        var ruta = RutaDe(referencia);
        return File.Exists(ruta) ? await File.ReadAllBytesAsync(ruta, ct) : null;
    }

    public Task EliminarAsync(string referencia, CancellationToken ct = default)
    {
        var ruta = RutaDe(referencia);
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExisteAsync(string referencia, CancellationToken ct = default) =>
        Task.FromResult(File.Exists(RutaDe(referencia)));

    private string RutaDe(string referencia) => Path.Combine(_raiz, Sanear(referencia));

    private static string Sanear(string nombre) => string.Concat(nombre.Split(Path.GetInvalidFileNameChars()));
}

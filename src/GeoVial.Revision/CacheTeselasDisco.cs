using System.Security.Cryptography;
using System.Text;

namespace GeoVial.Revision;

/// <summary>
/// Caché en disco de teselas del mapa para offline parcial en el móvil (US-21, paralelo al Service Worker de
/// la web): guarda los bytes de cada tesela en un archivo (clave = hash SHA-256 de la URL) bajo una carpeta,
/// con una capacidad máxima que descarta las entradas más viejas (FIFO por fecha de escritura). Lógica pura
/// sobre el sistema de archivos, testeable con una carpeta temporal. El interceptor del WebView del móvil la
/// usa para servir las teselas ya vistas sin red.
/// </summary>
public sealed class CacheTeselasDisco
{
    private const string Extension = ".tile";

    private readonly string _carpeta;
    private readonly int _capacidad;
    private readonly object _candado = new();

    public CacheTeselasDisco(string carpeta, int capacidad = 1000)
    {
        if (string.IsNullOrWhiteSpace(carpeta))
        {
            throw new ArgumentException("La carpeta de caché es obligatoria.", nameof(carpeta));
        }

        if (capacidad < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacidad), "La capacidad debe ser al menos 1.");
        }

        _carpeta = carpeta;
        _capacidad = capacidad;
        Directory.CreateDirectory(_carpeta);
    }

    public int Cantidad
    {
        get { lock (_candado) { return Directory.EnumerateFiles(_carpeta, "*" + Extension).Count(); } }
    }

    private string RutaDe(string url)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(url)));
        return Path.Combine(_carpeta, hash + Extension);
    }

    /// <summary>Devuelve los bytes cacheados de la tesela, o null si no está (o no se puede leer).</summary>
    public byte[]? Obtener(string url)
    {
        var ruta = RutaDe(url);
        lock (_candado)
        {
            if (!File.Exists(ruta))
            {
                return null;
            }

            try
            {
                return File.ReadAllBytes(ruta);
            }
            catch (IOException)
            {
                return null;
            }
        }
    }

    /// <summary>Guarda los bytes de la tesela; si se supera la capacidad, descarta las más viejas (FIFO).</summary>
    public void Guardar(string url, byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var ruta = RutaDe(url);
        lock (_candado)
        {
            File.WriteAllBytes(ruta, bytes);
            Recortar();
        }
    }

    private void Recortar()
    {
        var archivos = new DirectoryInfo(_carpeta).GetFiles("*" + Extension);
        if (archivos.Length <= _capacidad)
        {
            return;
        }

        foreach (var f in archivos.OrderBy(a => a.LastWriteTimeUtc).Take(archivos.Length - _capacidad))
        {
            try
            {
                f.Delete();
            }
            catch (IOException)
            {
                // si otro hilo/proceso ya lo borró o lo tiene tomado, se ignora
            }
        }
    }
}

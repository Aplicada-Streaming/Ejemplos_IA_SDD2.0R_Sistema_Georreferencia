namespace GeoVial.Revision;

/// <summary>
/// Caché en memoria de los binarios de fotos del carrusel de revisión (US-21/US-22), para no re-descargar la
/// foto en foco al navegar. Es una caché LRU: al exceder la capacidad máxima, descarta la entrada usada menos
/// recientemente. No es thread-safe; se usa desde el hilo de UI.
/// </summary>
public sealed class CacheFotos
{
    private readonly int _capacidad;
    private readonly Dictionary<Guid, LinkedListNode<Entrada>> _mapa = new();
    private readonly LinkedList<Entrada> _orden = new(); // el primero es el más recientemente usado

    private sealed record Entrada(Guid Clave, byte[] Datos);

    public CacheFotos(int capacidad = 8)
    {
        if (capacidad < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacidad), "La capacidad debe ser al menos 1.");
        }

        _capacidad = capacidad;
    }

    public int Cantidad => _mapa.Count;

    /// <summary>Devuelve los bytes cacheados de la foto (y la marca como recién usada), o null si no está.</summary>
    public byte[]? Obtener(Guid fotoId)
    {
        if (!_mapa.TryGetValue(fotoId, out var nodo))
        {
            return null;
        }

        _orden.Remove(nodo);
        _orden.AddFirst(nodo);
        return nodo.Value.Datos;
    }

    /// <summary>Guarda los bytes de la foto; si se supera la capacidad, descarta la entrada menos usada (LRU).</summary>
    public void Guardar(Guid fotoId, byte[] datos)
    {
        if (_mapa.TryGetValue(fotoId, out var existente))
        {
            existente.Value = new Entrada(fotoId, datos);
            _orden.Remove(existente);
            _orden.AddFirst(existente);
            return;
        }

        var nodo = new LinkedListNode<Entrada>(new Entrada(fotoId, datos));
        _orden.AddFirst(nodo);
        _mapa[fotoId] = nodo;

        if (_mapa.Count > _capacidad)
        {
            var menosUsada = _orden.Last!;
            _orden.RemoveLast();
            _mapa.Remove(menosUsada.Value.Clave);
        }
    }
}

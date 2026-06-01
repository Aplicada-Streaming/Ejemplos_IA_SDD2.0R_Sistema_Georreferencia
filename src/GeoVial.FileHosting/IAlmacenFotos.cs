namespace GeoVial.FileHosting;

/// <summary>
/// Abstracción del backend de almacenamiento de fotos (ADR-08, extensibilidad §2). La referencia es
/// opaca y estable; la base de datos persiste únicamente la referencia (<c>Foto.ReferenciaArchivo</c>,
/// modelo lógico §1.7), nunca el binario. Las implementaciones (local, S3, …) se seleccionan por
/// configuración del usuario raíz sin tocar el dominio ni el modelo de datos.
/// </summary>
public interface IAlmacenFotos
{
    /// <summary>Persiste el binario de la foto y devuelve una referencia estable y opaca.</summary>
    Task<string> GuardarAsync(string nombreSugerido, byte[] contenido, CancellationToken ct = default);

    /// <summary>Recupera el binario por su referencia; devuelve null si no existe.</summary>
    Task<byte[]?> RecuperarAsync(string referencia, CancellationToken ct = default);

    /// <summary>Elimina el binario por su referencia (idempotente: no falla si ya no existe).</summary>
    Task EliminarAsync(string referencia, CancellationToken ct = default);

    /// <summary>Indica si existe un binario con esa referencia.</summary>
    Task<bool> ExisteAsync(string referencia, CancellationToken ct = default);
}

using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using GeoVial.FileHosting;

namespace GeoVial.Infrastructure.Alojamiento;

/// <summary>
/// Backend de alojamiento sobre AWS S3 (ADR-08). Desacopla el archivo del ciclo de vida del contenedor
/// del backend (mitiga R-04). Recibe el cliente <see cref="IAmazonS3"/> por inyección para ser testeable
/// sin depender de una cuenta real. La base persiste solo la referencia (la clave del objeto en el bucket).
/// </summary>
public sealed class AlmacenS3 : IAlmacenFotos
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public AlmacenS3(IAmazonS3 s3, string bucket)
    {
        _s3 = s3;
        _bucket = bucket;
    }

    public async Task<string> GuardarAsync(string nombreSugerido, byte[] contenido, CancellationToken ct = default)
    {
        var clave = $"{Guid.NewGuid():N}-{nombreSugerido}";
        using var stream = new MemoryStream(contenido);
        await _s3.PutObjectAsync(new PutObjectRequest { BucketName = _bucket, Key = clave, InputStream = stream }, ct);
        return clave;
    }

    public async Task<byte[]?> RecuperarAsync(string referencia, CancellationToken ct = default)
    {
        try
        {
            using var respuesta = await _s3.GetObjectAsync(_bucket, referencia, ct);
            using var memoria = new MemoryStream();
            await respuesta.ResponseStream.CopyToAsync(memoria, ct);
            return memoria.ToArray();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task EliminarAsync(string referencia, CancellationToken ct = default) =>
        _s3.DeleteObjectAsync(_bucket, referencia, ct);

    public async Task<bool> ExisteAsync(string referencia, CancellationToken ct = default)
    {
        try
        {
            await _s3.GetObjectMetadataAsync(_bucket, referencia, ct);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}

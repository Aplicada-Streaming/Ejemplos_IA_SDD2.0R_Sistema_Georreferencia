using System.Net.Http.Headers;

namespace GeoVial.CapturaCampo;

/// <summary>
/// Arma el contenido multipart de la subida del binario de una foto al backend de alojamiento (BT-20, CU-04,
/// ADR-08). La parte se llama <c>archivo</c> para coincidir con el parámetro del endpoint de contenido
/// (<c>POST /fotos/{fotoId}/contenido</c>), lleva el nombre de archivo y los bytes con tipo de imagen.
/// </summary>
public static class ConstructorContenidoMultipart
{
    public const string NombreParte = "archivo";

    public static MultipartFormDataContent Construir(string nombreArchivo, byte[] bytes)
    {
        var parte = new ByteArrayContent(bytes);
        parte.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var contenido = new MultipartFormDataContent();
        contenido.Add(parte, NombreParte, nombreArchivo);
        return contenido;
    }
}

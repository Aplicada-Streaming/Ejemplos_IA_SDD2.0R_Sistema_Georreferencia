using Amazon.Runtime;
using Amazon.S3;
using FluentAssertions;
using GeoVial.Infrastructure.Alojamiento;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Prueba de <see cref="AlmacenS3"/> contra <b>LocalStack</b> (S3 emulado), para cubrir el comportamiento real
/// de AWS SDK v4 sin una cuenta AWS (cierra la brecha de verificación del Sprint 36, donde la migración a v4 se
/// validó por compilación + mocks). Se ejecuta sólo si la variable <c>LOCALSTACK_S3_URL</c> está configurada:
/// en CI hay un service container de LocalStack; localmente, sin Docker, la prueba hace no-op.
/// </summary>
public class AlmacenS3LocalStackTests
{
    private static string? Endpoint => Environment.GetEnvironmentVariable("LOCALSTACK_S3_URL");

    [Fact] // round-trip real contra S3 (LocalStack): guardar → existe → recuperar → 404 → eliminar
    public async Task Round_trip_contra_localstack()
    {
        var endpoint = Endpoint;
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return; // sin LocalStack (local, sin Docker): no-op. En CI corre completa.
        }

        using var s3 = new AmazonS3Client(
            new BasicAWSCredentials("test", "test"),
            new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true,          // LocalStack usa path-style (bucket en la ruta, no en el host)
                AuthenticationRegion = "us-east-1",
            });

        await EsperarListoAsync(s3);

        var bucket = $"geovial-test-{Guid.NewGuid():N}";
        await s3.PutBucketAsync(bucket);

        var almacen = new AlmacenS3(s3, bucket);
        var contenido = new byte[] { 1, 2, 3, 4, 5 };

        // Guardar devuelve una referencia y persiste el binario.
        var referencia = await almacen.GuardarAsync("foto.jpg", contenido);
        referencia.Should().EndWith("-foto.jpg");

        (await almacen.ExisteAsync(referencia)).Should().BeTrue();
        (await almacen.RecuperarAsync(referencia)).Should().Equal(contenido);

        // Un objeto inexistente: Recuperar → null, Existe → false (404 manejado).
        (await almacen.RecuperarAsync("no-existe")).Should().BeNull();
        (await almacen.ExisteAsync("no-existe")).Should().BeFalse();

        // Eliminar quita el binario.
        await almacen.EliminarAsync(referencia);
        (await almacen.ExisteAsync(referencia)).Should().BeFalse();
    }

    // LocalStack puede tardar en levantar: se reintenta la conexión hasta ~30 s antes de fallar.
    private static async Task EsperarListoAsync(IAmazonS3 s3)
    {
        for (var intento = 0; intento < 30; intento++)
        {
            try
            {
                await s3.ListBucketsAsync();
                return;
            }
            catch
            {
                await Task.Delay(1000);
            }
        }

        throw new InvalidOperationException("LocalStack no respondió a tiempo en LOCALSTACK_S3_URL.");
    }
}

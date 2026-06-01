using System.IO.Compression;
using System.Text;
using System.Text.Json;
using GeoVial.Application.Abstracciones;
using GeoVial.Application.ExportImport;

namespace GeoVial.Infrastructure.ExportImport;

/// <summary>
/// Empaqueta el manifiesto del relevamiento en un único archivo ZIP con una entrada `manifiesto.json`
/// (CU-08 §5.A/§5.B, contratos-rest §4.1). Los binarios de las fotos viven en la librería de alojamiento
/// configurable (ADR-08), fuera de este backend; el manifiesto transporta su referencia y fuente.
/// </summary>
public sealed class EmpaquetadorZip : IEmpaquetadorRelevamiento
{
    internal const string EntradaManifiesto = "manifiesto.json";
    internal const string CarpetaFotos = "fotos/";

    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    public byte[] Empaquetar(ManifiestoRelevamiento manifiesto, IReadOnlyDictionary<string, byte[]> binariosFotos)
    {
        using var memoria = new MemoryStream();
        using (var zip = new ZipArchive(memoria, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entrada = zip.CreateEntry(EntradaManifiesto, CompressionLevel.Optimal);
            using (var flujo = entrada.Open())
            {
                var json = JsonSerializer.SerializeToUtf8Bytes(manifiesto, Opciones);
                flujo.Write(json, 0, json.Length);
            }

            foreach (var (referencia, contenido) in binariosFotos)
            {
                var entradaFoto = zip.CreateEntry(CarpetaFotos + Uri.EscapeDataString(referencia), CompressionLevel.Optimal);
                using var flujoFoto = entradaFoto.Open();
                flujoFoto.Write(contenido, 0, contenido.Length);
            }
        }

        return memoria.ToArray();
    }

    public PaqueteRelevamiento? Desempaquetar(byte[] archivo)
    {
        try
        {
            using var memoria = new MemoryStream(archivo, writable: false);
            using var zip = new ZipArchive(memoria, ZipArchiveMode.Read);
            var entrada = zip.GetEntry(EntradaManifiesto);
            if (entrada is null)
            {
                return null;
            }

            ManifiestoRelevamiento? manifiesto;
            using (var flujo = entrada.Open())
            using (var lector = new StreamReader(flujo, Encoding.UTF8))
            {
                manifiesto = JsonSerializer.Deserialize<ManifiestoRelevamiento>(lector.ReadToEnd(), Opciones);
            }

            if (manifiesto is null)
            {
                return null;
            }

            var binarios = new Dictionary<string, byte[]>();
            foreach (var entradaFoto in zip.Entries)
            {
                if (!entradaFoto.FullName.StartsWith(CarpetaFotos, StringComparison.Ordinal) || entradaFoto.FullName.Length == CarpetaFotos.Length)
                {
                    continue;
                }

                var referencia = Uri.UnescapeDataString(entradaFoto.FullName[CarpetaFotos.Length..]);
                using var flujoFoto = entradaFoto.Open();
                using var memoriaFoto = new MemoryStream();
                flujoFoto.CopyTo(memoriaFoto);
                binarios[referencia] = memoriaFoto.ToArray();
            }

            return new PaqueteRelevamiento(manifiesto, binarios);
        }
        catch (Exception ex) when (ex is InvalidDataException or JsonException or ArgumentException)
        {
            // Archivo corrupto, no es un ZIP, o el manifiesto no es JSON válido: lo trata el handler como inválido.
            return null;
        }
    }
}

using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.ExportImport;

/// <summary>Commands de exportación e importación del relevamiento completo (CU-08 §5.A/§5.B; US-27/US-28).</summary>

/// <summary>US-27 / CU-08 §5.A: exporta el relevamiento completo a un único archivo ZIP.</summary>
public sealed record ExportarRelevamientoCommand(Guid UsuarioId, Guid RelevamientoId)
    : IPeticion<Resultado<ArchivoExportado>>;

/// <summary>US-28 / CU-08 §5.B: importa el relevamiento completo desde un archivo ZIP. Devuelve el nuevo identificador.</summary>
public sealed record ImportarRelevamientoCommand(Guid UsuarioId, byte[] Archivo)
    : IPeticion<Resultado<Guid>>;

/// <summary>Archivo de exportación entregable (nombre sugerido y contenido binario del ZIP).</summary>
public sealed record ArchivoExportado(string NombreArchivo, byte[] Contenido);

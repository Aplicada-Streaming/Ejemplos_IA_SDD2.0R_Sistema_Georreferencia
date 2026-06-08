namespace GeoVial.Application.Configuracion;

/// <summary>
/// Política de migración de la base al desplegar (hardening). En **Development** el backend auto-migra al
/// arrancar (conveniencia, instancia única). En **producción** NO: aplicar migraciones desde varias réplicas a la
/// vez es frágil; la migración se hace como un **paso explícito del despliegue** (un job / init-container que
/// invoca el proceso con el comando <see cref="ComandoMigrar"/>, aplica las migraciones y termina), antes de
/// levantar las instancias que sirven tráfico —y el readiness (<c>/health/ready</c>) no da OK mientras haya
/// migraciones pendientes—. Lógica pura y testeable; <c>Program</c> sólo la consulta.
/// </summary>
public static class PoliticaMigracion
{
    /// <summary>Primer argumento que pone al proceso en modo "migrar y salir" (paso de despliegue).</summary>
    public const string ComandoMigrar = "migrate";

    /// <summary>El backend auto-migra al arrancar sólo en Development (instancia única, conveniencia).</summary>
    public static bool DebeAutoMigrarAlArrancar(string entorno) =>
        string.Equals(entorno, "Development", StringComparison.OrdinalIgnoreCase);

    /// <summary>El proceso fue invocado para migrar y salir: el primer argumento es <see cref="ComandoMigrar"/>.</summary>
    public static bool EsComandoMigrar(string[] args) =>
        args.Length > 0 && string.Equals(args[0], ComandoMigrar, StringComparison.OrdinalIgnoreCase);
}

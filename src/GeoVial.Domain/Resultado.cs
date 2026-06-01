namespace GeoVial.Domain;

/// <summary>
/// Resultado de una operación que puede fallar con un código de error de dominio,
/// sin recurrir a excepciones para los flujos esperados (CU-02, CU-03, CU-14).
/// </summary>
public sealed class Resultado
{
    public bool EsExito { get; }
    public string? Codigo { get; }

    private Resultado(bool esExito, string? codigo)
    {
        EsExito = esExito;
        Codigo = codigo;
    }

    public static Resultado Exito() => new(true, null);
    public static Resultado Fallo(string codigo) => new(false, codigo);
}

/// <summary>Resultado con valor de retorno en caso de éxito.</summary>
public sealed class Resultado<T>
{
    public bool EsExito { get; }
    public string? Codigo { get; }
    public T? Valor { get; }

    private Resultado(bool esExito, string? codigo, T? valor)
    {
        EsExito = esExito;
        Codigo = codigo;
        Valor = valor;
    }

    public static Resultado<T> Exito(T valor) => new(true, null, valor);
    public static Resultado<T> Fallo(string codigo) => new(false, codigo, default);
}

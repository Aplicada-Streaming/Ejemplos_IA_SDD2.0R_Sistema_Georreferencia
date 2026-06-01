using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Application.Cqrs;

/// <summary>
/// CQRS ligero (PROJECT-README §3, ADR-01) para el módulo de relevamientos. Una petición (command o
/// query) se despacha a su único manejador resuelto por inyección de dependencias. Sin librería externa.
/// </summary>
public interface IPeticion<TResultado>
{
}

public interface IManejador<in TPeticion, TResultado>
    where TPeticion : IPeticion<TResultado>
{
    Task<TResultado> ManejarAsync(TPeticion peticion, CancellationToken ct = default);
}

public interface IMediador
{
    Task<TResultado> EnviarAsync<TResultado>(IPeticion<TResultado> peticion, CancellationToken ct = default);
}

public sealed class Mediador : IMediador
{
    private readonly IServiceProvider _proveedor;

    public Mediador(IServiceProvider proveedor) => _proveedor = proveedor;

    public async Task<TResultado> EnviarAsync<TResultado>(IPeticion<TResultado> peticion, CancellationToken ct = default)
    {
        var tipoManejador = typeof(IManejador<,>).MakeGenericType(peticion.GetType(), typeof(TResultado));
        var manejador = _proveedor.GetRequiredService(tipoManejador);
        var metodo = tipoManejador.GetMethod(nameof(IManejador<IPeticion<TResultado>, TResultado>.ManejarAsync))!;
        var tarea = (Task<TResultado>)metodo.Invoke(manejador, new object[] { peticion, ct })!;
        return await tarea;
    }
}

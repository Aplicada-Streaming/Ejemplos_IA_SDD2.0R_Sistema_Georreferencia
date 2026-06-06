using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Marca de "captura en curso" (S55) en <see cref="Preferences"/> (no sensible, robusto en arranque). Se setea
/// antes de abrir la cámara y se limpia al volver; si el SO mata el proceso mientras la cámara está en primer
/// plano, la marca sobrevive y el arranque sabe que "vuelve de la cámara" y no debe re-pedir el método.
/// </summary>
public sealed class MarcadorCapturaPreferences : IMarcadorCaptura
{
    private const string Clave = "geovial.captura.en-curso";

    public Task MarcarEnCursoAsync()
    {
        Preferences.Default.Set(Clave, true);
        return Task.CompletedTask;
    }

    public Task<bool> EstaEnCursoAsync() => Task.FromResult(Preferences.Default.Get(Clave, false));

    public Task LimpiarAsync()
    {
        Preferences.Default.Remove(Clave);
        return Task.CompletedTask;
    }
}

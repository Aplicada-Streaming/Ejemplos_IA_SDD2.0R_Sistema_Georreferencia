#if ANDROID
using Android.Webkit;
using GeoVial.Revision;

namespace GeoVial.Mobile;

/// <summary>
/// WebViewClient (Android) que da offline parcial al mapa del móvil (US-21): intercepta las peticiones de
/// teselas de OpenStreetMap y las sirve desde <see cref="CacheTeselasDisco"/> si están; si no, las descarga,
/// las guarda en la caché y las devuelve. El resto de las peticiones pasan de largo. Así, una zona ya vista
/// se muestra sin red (paralelo al Service Worker de la web del Sprint 34). Fuera de CI (plataforma Android).
/// </summary>
public sealed class MapaWebViewClient : WebViewClient
{
    private static readonly HttpClient Http = CrearCliente();
    private readonly CacheTeselasDisco _cache;

    // OSM bloquea (403 "tile usage policy") las peticiones sin un User-Agent que identifique la app.
    // El navegador de la web manda el suyo; el fetch nativo de Android no, así que lo seteamos acá.
    private static HttpClient CrearCliente()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "GeoVial/1.0 (relevamiento vial; +https://github.com/fernandofilipuzzi-utn)");
        return http;
    }

    public MapaWebViewClient(CacheTeselasDisco cache) => _cache = cache;

    public override WebResourceResponse? ShouldInterceptRequest(Android.Webkit.WebView? view, IWebResourceRequest? request)
    {
        var url = request?.Url?.ToString();
        if (url is null || !MapaTeselas.EsUrlDeTesela(url))
        {
            return base.ShouldInterceptRequest(view, request); // sólo se interceptan las teselas de OSM
        }

        try
        {
            var bytes = _cache.Obtener(url);
            if (bytes is null)
            {
                // ShouldInterceptRequest corre fuera del hilo de UI: la descarga sincrónica es aceptable.
                bytes = Http.GetByteArrayAsync(url).GetAwaiter().GetResult();
                _cache.Guardar(url, bytes);
            }

            return new WebResourceResponse("image/png", "binary", new MemoryStream(bytes));
        }
        catch
        {
            // Sin red y sin caché para esta tesela: que el WebView maneje el fallo (tesela en blanco).
            return base.ShouldInterceptRequest(view, request);
        }
    }
}
#endif

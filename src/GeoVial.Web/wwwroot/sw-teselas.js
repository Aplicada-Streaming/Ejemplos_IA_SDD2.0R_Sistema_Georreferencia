// Service Worker: cachea las teselas de OpenStreetMap ya visitadas para verlas sin red (offline parcial,
// US-21 / mapa-offline-teselas). Cachea exactamente el host de OSM (mismo contrato que
// GeoVial.Revision.MapaTeselas.EsUrlDeTesela), con un límite de entradas (FIFO) para no crecer sin control.
// No cachea nada más: la librería Leaflet ya está vendorizada y los datos de la app van por su propio canal.

const CACHE = "geovial-teselas-v1";
const HOST = "tile.openstreetmap.org";
const MAX = 500; // ~ varias pantallas de mapa; acota el uso de almacenamiento

function esTesela(url) {
  try {
    const u = new URL(url);
    return u.host === HOST && u.pathname.toLowerCase().endsWith(".png");
  } catch (e) {
    return false;
  }
}

self.addEventListener("install", () => self.skipWaiting());
self.addEventListener("activate", (event) => event.waitUntil(self.clients.claim()));

self.addEventListener("fetch", (event) => {
  const req = event.request;
  if (req.method !== "GET" || !esTesela(req.url)) {
    return; // sólo intercepta teselas de OSM; el resto sigue su curso normal
  }

  event.respondWith((async () => {
    const cache = await caches.open(CACHE);
    const cacheado = await cache.match(req);
    if (cacheado) {
      return cacheado; // cache-first: si ya se vio la tesela, funciona sin red
    }

    try {
      const resp = await fetch(req);
      // Las teselas son peticiones no-cors (respuesta 'opaque', status 0): se cachean igual.
      if (resp && (resp.ok || resp.type === "opaque")) {
        await cache.put(req, resp.clone());
        trim(cache); // recorte en segundo plano (sin await)
      }
      return resp;
    } catch (e) {
      // Sin red y sin caché para esta tesela: error controlado (el <img> de Leaflet lo maneja).
      return cacheado || Response.error();
    }
  })());
});

// FIFO: las claves vienen en orden de inserción; se borran las más viejas hasta el límite.
async function trim(cache) {
  const claves = await cache.keys();
  const sobrante = claves.length - MAX;
  for (let i = 0; i < sobrante; i++) {
    await cache.delete(claves[i]);
  }
}

// Mapa interactivo de la revisión (US-21) con Leaflet + teselas de OpenStreetMap.
// OSM es libre: no requiere clave de proveedor; sólo se incluye la atribución obligatoria.
// La página Blazor llama a render(elementId, vista) tras cargar la revisión, y a destruir() al disponer.

let mapa = null;
let capaMarcadores = null;

export function render(elementId, vista) {
    if (typeof L === "undefined") {
        console.error("Leaflet no está cargado.");
        return;
    }

    // Leaflet está vendorizado en wwwroot/lib/leaflet (Sprint 31); fijamos la ruta de los íconos de marcador
    // para que resuelvan desde ahí (sin esto, Leaflet la autodetecta del CSS y puede fallar al vendorizar).
    L.Icon.Default.imagePath = "lib/leaflet/images/";

    if (!mapa) {
        mapa = L.map(elementId);
        L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        }).addTo(mapa);
        capaMarcadores = L.layerGroup().addTo(mapa);
    }

    capaMarcadores.clearLayers();

    if (!vista || !vista.pins || vista.pins.length === 0) {
        mapa.setView([vista ? vista.centroLat : 0, vista ? vista.centroLon : 0], 4);
        mapa.invalidateSize();
        return;
    }

    for (const p of vista.pins) {
        const popup =
            `<strong>Marcador</strong> (${p.latitud.toFixed(5)}, ${p.longitud.toFixed(5)})` +
            (p.enConflicto ? ' &mdash; <span style="color:#c0392b">en conflicto</span>' : "") +
            `<br>Fotos: ${p.fotos} &middot; Comentarios: ${p.comentarios}`;
        L.marker([p.latitud, p.longitud]).bindPopup(popup).addTo(capaMarcadores);
    }

    if (vista.pins.length === 1) {
        mapa.setView([vista.pins[0].latitud, vista.pins[0].longitud], 16);
    } else {
        mapa.fitBounds(
            [[vista.minLat, vista.minLon], [vista.maxLat, vista.maxLon]],
            { padding: [30, 30] });
    }

    // El contenedor puede haber cambiado de tamaño tras el render de Blazor.
    mapa.invalidateSize();
}

export function destruir() {
    if (mapa) {
        mapa.remove();
        mapa = null;
        capaMarcadores = null;
    }
}

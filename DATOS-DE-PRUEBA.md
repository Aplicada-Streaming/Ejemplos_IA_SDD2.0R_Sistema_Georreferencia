# GeoVial — Usuarios y datos de prueba

Credenciales y datos de prueba para probar la API y la app móvil en **desarrollo**.

> ⚠️ **El backend de desarrollo usa base en memoria (InMemory).** Sólo persisten mientras el proceso del backend está vivo. Al reiniciarlo, vuelven **únicamente** el usuario `raiz` y el relevamiento demo (los crea el seed de arranque). El resto de los usuarios y relevamientos de esta tabla se recrean corriendo el script de la sección [§4](#4-cómo-recrear-los-datos-tras-reiniciar-el-backend).
>
> En un entorno con SQL Server (connection string `GeoVial`) los datos persisten entre reinicios.

## 1. Usuarios

Todas las claves siguen el formato de demo (no son productivas).

| Usuario | Clave | Rol | Área | Nombre | Para qué sirve |
| --- | --- | --- | --- | --- | --- |
| `raiz` | `GeoVial.Raiz.2026` | Raíz | — | Administrador técnico | Ve todo; crea el jefe general (seed de arranque) |
| `jefe.general` | `JefeGeneral.2026` | Jefe General | — | Jorge | Ve todas las áreas; administra jefes de área |
| `jefe.norte` | `JefeNorte.2026` | Jefe de Área | Zona Norte | Norma | Crea/asigna relevamientos de su área; revisa |
| `campo1` | `Campo1.2026` | Agente de Campo | Zona Norte | Carlos | Agente con 2 relevamientos asignados |
| `campo2` | `Campo2.2026` | Agente de Campo | Zona Norte | Ana | Agente con 2 relevamientos asignados |
| `campo3` | `Campo3.2026` | Agente de Campo | Zona Norte | Diego | Agente **sin** asignaciones (probar lista vacía) |

> Sólo existe un área sembrada (**Zona Norte**); no hay endpoint para crear áreas (las crea el seed). Por eso todos los usuarios de área pertenecen a Zona Norte.

### Jerarquía (RN-01: cada nivel administra sólo a su inferior inmediato)

```
Raíz (raiz)
 └─ Jefe General (jefe.general)
     └─ Jefe de Área — Zona Norte (jefe.norte)
         ├─ Agente campo1 (Carlos)
         ├─ Agente campo2 (Ana)
         └─ Agente campo3 (Diego)
```

## 2. Relevamientos (todos en Zona Norte)

| Obra | Estado | Agentes asignados |
| --- | --- | --- |
| Obra demo — Puente Río 12 | Recolección | `campo1` |
| Ruta 8 km 45 — alcantarilla | Recolección | `campo2` |
| Túnel Acceso Sur — fisuras | Recolección | `campo1`, `campo2` |

Resultado esperado de **"relevamientos asignados a mí"** (`GET /relevamientos/mios`):

- `campo1` → Puente Río 12 + Túnel Acceso Sur
- `campo2` → Ruta 8 + Túnel Acceso Sur
- `campo3` → (vacío)
- `jefe.norte` / `jefe.general` / `raiz` → ven todos por el listado de área (`GET /relevamientos`)

## 3. Cómo levantar el entorno

**Backend** (puerto 5080, entorno Development):

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5080"
dotnet run --project src/GeoVial.Api -c Debug --no-launch-profile
```

**App móvil (Android)** — el dispositivo alcanza el backend por `adb reverse`:

```powershell
$env:ANDROID_HOME = "C:/Program Files (x86)/Android/android-sdk"
adb reverse tcp:5080 tcp:5080
dotnet build src/GeoVial.Mobile/GeoVial.Mobile.csproj -c Release -f net10.0-android `
  -r android-arm64 -p:AndroidSdkDirectory="$env:ANDROID_HOME" -t:Run
```

## 4. Cómo recrear los datos tras reiniciar el backend

Con el backend corriendo, guardá esto como `seed-datos-prueba.py` y ejecutá `python seed-datos-prueba.py`. Es idempotente respecto del `raiz` y el relevamiento demo (los aporta el seed); crea el resto.

```python
import json, urllib.request as u
B = "http://localhost:5080/api/v1"

def call(m, p, t=None, b=None):
    d = json.dumps(b).encode() if b is not None else None
    r = u.Request(B + p, data=d, method=m); r.add_header("Content-Type", "application/json")
    if t: r.add_header("Authorization", "Bearer " + t)
    try:
        with u.urlopen(r) as x:
            raw = x.read().decode(); return x.status, (json.loads(raw) if raw.strip() else None)
    except u.HTTPError as e:
        return e.code, e.read().decode()[:160]

def login(usr, cl): return call("POST", "/auth/login", b={"nombreUsuario": usr, "clave": cl})[1]["accessToken"]
def alta(tok, nombre, rol, area): s, b = call("POST", "/usuarios", tok, {"nombre": nombre, "rol": rol, "areaId": area}); return b["usuarioId"]
def cred(tok, uid, usr, cl): call("POST", f"/usuarios/{uid}/credencial", tok, {"nombreUsuario": usr, "clave": cl})

raiz = login("raiz", "GeoVial.Raiz.2026")
rels = call("GET", "/relevamientos", raiz)[1]
areaNorte = rels[0]["areaId"]; demoRel = rels[0]["relevamientoId"]

jg = alta(raiz, "Jorge (Jefe General)", 2, None); cred(raiz, jg, "jefe.general", "JefeGeneral.2026")
jgt = login("jefe.general", "JefeGeneral.2026")
ja = alta(jgt, "Norma (Jefa de Zona Norte)", 3, areaNorte); cred(jgt, ja, "jefe.norte", "JefeNorte.2026")
jat = login("jefe.norte", "JefeNorte.2026")

ag = {}
for nom, usr, cl in [("Carlos (Agente)", "campo1", "Campo1.2026"),
                     ("Ana (Agente)", "campo2", "Campo2.2026"),
                     ("Diego (Agente)", "campo3", "Campo3.2026")]:
    uid = alta(jat, nom, 4, areaNorte); cred(jat, uid, usr, cl); ag[usr] = uid

def crear(obra, radio): return call("POST", "/relevamientos", jat, {"identificacionObra": obra, "radioAgrupacionMetros": radio})[1]["relevamientoId"]
def asignar(rel, uids): call("POST", f"/relevamientos/{rel}/agentes", jat, {"agentesIds": uids})

r2 = crear("Ruta 8 km 45 — alcantarilla", 15)
r3 = crear("Túnel Acceso Sur — fisuras", 20)
asignar(demoRel, [ag["campo1"]])
asignar(r2, [ag["campo2"]])
asignar(r3, [ag["campo1"], ag["campo2"]])
print("Datos de prueba creados.")
```

Roles (`rol`): `1` = Raíz · `2` = Jefe General · `3` = Jefe de Área · `4` = Agente de Campo.

## 5. Notas

- **Reingreso en terreno (RN-06):** tras un login exitoso en un teléfono con seguridad configurada (huella / rostro / **patrón** / PIN), la app recuerda el usuario y ofrece "Reingreso en terreno" sin clave, verificando con el método nativo del dispositivo.
- **Bandeja sin georreferenciar:** capturá una foto **sin GPS** (o sembrá una observación sin coordenadas) para que aparezca en la solapa *Bandeja* y poder ubicarla en el mapa.
- Estas credenciales son **sólo para desarrollo/demo**; no usar en producción.

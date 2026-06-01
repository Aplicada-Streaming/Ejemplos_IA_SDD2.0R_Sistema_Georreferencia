# Scripts de desarrollo local — GeoVial

Scripts BAT para Windows (README §11, §16: desarrollo íntegramente local). El host de SQL Server local se llama **DEV**.

| Script | Qué hace |
|---|---|
| `db-create.bat` | Crea/actualiza la base `GeoVial` en el host `DEV` aplicando las migraciones de EF Core (ADR-09). |
| `run-backend.bat` | Levanta la API REST en `http://localhost:5080` contra SQL Server en `DEV`. Al arrancar migra y siembra el usuario raíz (BT-10). |
| `run-frontend.bat` | Levanta el front web Blazor en `http://localhost:5180` consumiendo la API. |
| `run-all.bat` | Levanta backend y front en ventanas separadas. |
| `build.bat` | Compila la solución (sin warnings como error) y corre la suite de tests. |

## Primer uso

```bat
scripts\db-create.bat
scripts\run-all.bat
```

Luego abrir `http://localhost:5180/usuarios` e iniciar sesión con el usuario `raiz` (clave inicial `GeoVial.Raiz.2026`, cambiar en cuanto se opere).

## Sin SQL Server local

Para correr el backend con base en memoria (sin host `DEV`):

```bat
dotnet run --project src/GeoVial.Api
```

## Configuración del host de base

La cadena por defecto es `Server=DEV;Database=GeoVial;Trusted_Connection=True;TrustServerCertificate=True`.
Se puede sobrescribir con la variable de entorno `GEOVIAL_DB` (migraciones) o `ConnectionStrings__GeoVial` (API).

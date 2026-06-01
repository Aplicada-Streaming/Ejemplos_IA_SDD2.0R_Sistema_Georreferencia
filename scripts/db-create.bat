@echo off
rem ============================================================================
rem GeoVial - Creacion / actualizacion de la base de datos local (host DEV)
rem Aplica las migraciones de EF Core (ADR-09) sobre SQL Server en el host DEV.
rem Requiere: SQL Server accesible como "DEV" y la herramienta dotnet-ef
rem (se restaura sola con "dotnet tool restore").
rem ============================================================================
setlocal
set "GEOVIAL_DB=Server=DEV;Database=GeoVial;Trusted_Connection=True;TrustServerCertificate=True"

pushd "%~dp0.."

echo Restaurando herramientas locales (dotnet-ef)...
dotnet tool restore
if errorlevel 1 goto :error

echo Creando/actualizando la base GeoVial en el host DEV...
dotnet ef database update --project src/GeoVial.Infrastructure --startup-project src/GeoVial.Api
if errorlevel 1 goto :error

echo.
echo Base GeoVial lista en DEV. Usuario raiz inicial: "raiz" (se siembra al arrancar el backend).
popd
endlocal
exit /b 0

:error
echo.
echo ERROR: no se pudo crear/actualizar la base. Verifique que SQL Server "DEV" este accesible.
popd
endlocal
exit /b 1

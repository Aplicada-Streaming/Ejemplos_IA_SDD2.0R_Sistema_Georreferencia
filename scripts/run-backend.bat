@echo off
rem ============================================================================
rem GeoVial - Backend (API REST) local contra SQL Server en el host DEV.
rem Escucha en http://localhost:5080. Al arrancar aplica migraciones y siembra
rem el usuario raiz (BT-10). Para correr sin SQL Server (base en memoria),
rem ejecute en su lugar: dotnet run --project src/GeoVial.Api
rem ============================================================================
setlocal
set "ASPNETCORE_ENVIRONMENT=Development"
set "ConnectionStrings__GeoVial=Server=DEV;Database=GeoVial;Trusted_Connection=True;TrustServerCertificate=True"

pushd "%~dp0.."
echo Backend GeoVial en http://localhost:5080  (base SQL Server: DEV)
dotnet run --project src/GeoVial.Api --urls http://localhost:5080
popd
endlocal

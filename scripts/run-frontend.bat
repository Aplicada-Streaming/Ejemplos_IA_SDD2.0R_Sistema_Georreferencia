@echo off
rem ============================================================================
rem GeoVial - Front web (Blazor Interactive Server) local.
rem Escucha en http://localhost:5180 y consume la API en http://localhost:5080.
rem Pagina del slice del Sprint 01: http://localhost:5180/usuarios
rem ============================================================================
setlocal
set "ASPNETCORE_ENVIRONMENT=Development"
set "ApiBaseUrl=http://localhost:5080/"

pushd "%~dp0.."
echo Front web GeoVial en http://localhost:5180  (API: %ApiBaseUrl%)
dotnet run --project src/GeoVial.Web --urls http://localhost:5180
popd
endlocal

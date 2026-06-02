@echo off
REM STAGE-13 (publish) - Publica el paquete GeoVial.Sync a GitHub Packages.
REM Requiere la variable GH_PACKAGES_TOKEN (scope write:packages, desde GitHub Secrets; prohibido el commit)
REM y la organizacion en GH_ORG. Ver guia-publicacion-paquete-github-packages §2.
if "%GH_PACKAGES_TOKEN%"=="" ( echo ERROR: falta GH_PACKAGES_TOKEN & exit /b 1 )
if "%GH_ORG%"=="" ( set GH_ORG=Aplicada-Streaming )
for %%f in (artifacts\GeoVial.Sync.*.nupkg) do (
  dotnet nuget push "%%f" --source "https://nuget.pkg.github.com/%GH_ORG%/index.json" --api-key %GH_PACKAGES_TOKEN% --skip-duplicate
)

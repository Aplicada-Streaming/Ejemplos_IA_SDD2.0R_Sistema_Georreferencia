@echo off
REM STAGE-12 (build) - Construye localmente las tres imagenes Docker del monolito GeoVial
REM (front/backend/db) con el mismo contexto y Dockerfiles que el workflow publish-images.yml.
REM Equivalente documentado en guia-publicacion-image-docker §2.
REM
REM Variables: IMAGE_VERSION (version SemVer; por defecto "dev"), IMAGE_REGISTRY (por defecto
REM   ghcr.io/aplicada-streaming). Requiere Docker con Buildx.
setlocal
if "%IMAGE_VERSION%"=="" set "IMAGE_VERSION=dev"
if "%IMAGE_REGISTRY%"=="" set "IMAGE_REGISTRY=ghcr.io/aplicada-streaming"

pushd "%~dp0.."
echo Construyendo imagenes GeoVial %IMAGE_VERSION% en %IMAGE_REGISTRY% ...

docker build -f src/GeoVial.Api/Dockerfile -t %IMAGE_REGISTRY%/geovial-backend:%IMAGE_VERSION% .
if errorlevel 1 goto :error
docker build -f src/GeoVial.Web/Dockerfile -t %IMAGE_REGISTRY%/geovial-front:%IMAGE_VERSION% .
if errorlevel 1 goto :error
docker build -f infra/db/Dockerfile -t %IMAGE_REGISTRY%/geovial-db:%IMAGE_VERSION% .
if errorlevel 1 goto :error

echo.
echo Imagenes construidas: geovial-backend, geovial-front, geovial-db (tag %IMAGE_VERSION%).
popd
endlocal
exit /b 0

:error
echo.
echo ERROR: fallo el build de una imagen. Verifique que Docker este disponible.
popd
endlocal
exit /b 1

@echo off
REM STAGE-14 (publish) - Publica a GHCR las tres imagenes Docker del monolito GeoVial.
REM En CI lo hace el workflow publish-images.yml (build + SBOM + firma + push) al taggear v*; este
REM script es el equivalente manual reproducible (guia-publicacion-image-docker §2). La firma cosign
REM y el SBOM se aplican en el workflow; el push manual NO los reemplaza.
REM
REM Variables: IMAGE_VERSION (obligatoria), IMAGE_REGISTRY (por defecto ghcr.io/aplicada-streaming).
REM   Antes de publicar: echo %GHCR_TOKEN% ^| docker login ghcr.io -u ^<usuario^> --password-stdin
setlocal
if "%IMAGE_VERSION%"=="" (
  echo ERROR: defina IMAGE_VERSION con la version SemVer a publicar.
  endlocal
  exit /b 1
)
if "%IMAGE_REGISTRY%"=="" set "IMAGE_REGISTRY=ghcr.io/aplicada-streaming"

pushd "%~dp0.."
echo Publicando imagenes GeoVial %IMAGE_VERSION% a %IMAGE_REGISTRY% ...

docker push %IMAGE_REGISTRY%/geovial-backend:%IMAGE_VERSION%
if errorlevel 1 goto :error
docker push %IMAGE_REGISTRY%/geovial-front:%IMAGE_VERSION%
if errorlevel 1 goto :error
docker push %IMAGE_REGISTRY%/geovial-db:%IMAGE_VERSION%
if errorlevel 1 goto :error

echo.
echo Imagenes publicadas. Recuerde: la firma cosign y el SBOM se generan en el workflow publish-images.yml.
popd
endlocal
exit /b 0

:error
echo.
echo ERROR: fallo el push de una imagen. Verifique el docker login a GHCR y los permisos del token.
popd
endlocal
exit /b 1

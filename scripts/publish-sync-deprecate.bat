@echo off
REM Rollback (guia-publicacion-paquete-github-packages §4): marca una version como no listada/deprecada.
REM No se "revierte" una version publicada: se desaconseja y se publica un PATCH con el fix.
REM Uso: publish-sync-deprecate.bat ^<version-rota^> "^<motivo; usar version-recomendada^>"
if "%~1"=="" ( echo Uso: publish-sync-deprecate.bat ^<version-rota^> "^<motivo^>" & exit /b 1 )
echo Deprecando GeoVial.Sync %1 : %~2
echo.
echo Operacion manual en GitHub Packages:
echo   1. Marcar la version %1 como NO listada (unlist) en el feed.
echo   2. Publicar un PATCH con el fix por el flujo normal (tag v^<X.Y.Z+1^>).
echo   3. Registrar la deprecacion en src/GeoVial.Sync/CHANGELOG.md (seccion Deprecated/Removed).

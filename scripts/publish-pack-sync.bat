@echo off
REM STAGE-11 (package) - Empaqueta GeoVial.Sync como .nupkg en ./artifacts.
REM MinVer calcula la version desde el tag Git con prefijo v (estrategia-versionado).
REM Equivalente documentado en guia-publicacion-paquete-github-packages §2.
dotnet pack src/GeoVial.Sync -c Release -o ./artifacts

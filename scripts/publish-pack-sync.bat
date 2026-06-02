@echo off
REM STAGE-11 (package) - Empaqueta GeoVial.Sync como .nupkg en ./artifacts.
REM MinVer calcula la version desde el tag Git con prefijo v (estrategia-versionado).
REM Equivalente documentado en guia-publicacion-paquete-github-packages §2.
REM GeneratePackageOnBuild=false en el pack explicito: evita NU5026 en checkout limpio (la DLL no se
REM encuentra) por la interaccion entre GeneratePackageOnBuild y 'dotnet pack'. No modifica el .csproj.
dotnet pack src/GeoVial.Sync -c Release -o ./artifacts -p:GeneratePackageOnBuild=false

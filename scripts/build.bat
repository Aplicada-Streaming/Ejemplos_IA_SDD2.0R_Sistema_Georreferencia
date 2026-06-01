@echo off
rem ============================================================================
rem GeoVial - Compilacion y pruebas de toda la solucion.
rem Build sin warnings tratados como error + suite de tests (DoD de 08).
rem ============================================================================
setlocal
pushd "%~dp0.."

echo Compilando la solucion...
dotnet build
if errorlevel 1 goto :error

echo Ejecutando pruebas...
dotnet test --nologo
if errorlevel 1 goto :error

echo.
echo Build y tests OK.
popd
endlocal
exit /b 0

:error
echo.
echo ERROR: la compilacion o las pruebas fallaron.
popd
endlocal
exit /b 1

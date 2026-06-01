@echo off
rem ============================================================================
rem GeoVial - Levanta backend y front en ventanas separadas (host local DEV).
rem Orden sugerido la primera vez:
rem   1) scripts\db-create.bat   (crea la base en DEV)
rem   2) scripts\run-all.bat     (backend + front)
rem ============================================================================
setlocal
echo Levantando backend y front de GeoVial...
start "GeoVial Backend"  cmd /k "%~dp0run-backend.bat"
timeout /t 6 /nobreak >nul
start "GeoVial Frontend" cmd /k "%~dp0run-frontend.bat"
echo.
echo Backend:  http://localhost:5080
echo Front:    http://localhost:5180/usuarios
endlocal

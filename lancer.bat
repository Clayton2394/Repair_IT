@echo off
chcp 65001 > nul
setlocal
cd /d "%~dp0"

echo ==========================================
echo   RepairIT - lancement de tous les projets
echo ==========================================

echo [1/3] Compilation de la solution...
dotnet build RepairIT.sln -v q --nologo
if errorlevel 1 (
    echo La compilation a echoue. Lancement annule.
    pause
    exit /b 1
)

echo [2/3] Demarrage des microservices (une fenetre par service)...
start "RepairIT.CatalogService  :5101" cmd /k dotnet run --no-build --project src\RepairIT.CatalogService
start "RepairIT.RepairService   :5103" cmd /k dotnet run --no-build --project src\RepairIT.RepairService
start "RepairIT.SalesService    :5104" cmd /k dotnet run --no-build --project src\RepairIT.SalesService
start "RepairIT.AuthService     :5105" cmd /k dotnet run --no-build --project src\RepairIT.AuthentificationService

echo [3/3] Demarrage du front Blazor...
start "RepairIT.Client          :5000" cmd /k dotnet run --no-build --project src\RepairIT.Client

echo.
echo Front       : http://localhost:5000
echo Catalogue   : http://localhost:5101/api/ordinateurs
echo.
echo Fermez les fenetres ouvertes pour tout arreter.

timeout /t 8 > nul
start "" http://localhost:5000
endlocal

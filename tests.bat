@echo off
chcp 65001 > nul
setlocal
cd /d "%~dp0"

echo Execution des tests unitaires avec couverture de code...
if exist TestResults rmdir /s /q TestResults

dotnet test RepairIT.sln --collect:"XPlat Code Coverage" --results-directory TestResults
if errorlevel 1 (
    echo Des tests ont echoue.
    exit /b 1
)

dotnet tool restore > nul
dotnet reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:TestResults/rapport -reporttypes:"Html;TextSummary" > nul

type TestResults\rapport\Summary.txt
echo.
echo Rapport HTML : TestResults\rapport\index.html
endlocal

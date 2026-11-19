@echo off
chcp 65001 > nul
echo Arret de tous les projets RepairIT...

rem Les APIs tournent sous leur propre nom de processus
taskkill /F /IM RepairIT.CatalogService.exe /T > nul 2>&1
taskkill /F /IM RepairIT.UserService.exe /T > nul 2>&1
taskkill /F /IM RepairIT.RepairService.exe /T > nul 2>&1
taskkill /F /IM RepairIT.SalesService.exe /T > nul 2>&1
taskkill /F /IM RepairIT.AuthentificationService.exe /T > nul 2>&1

rem Le serveur de developpement Blazor tourne sous "dotnet" : on libere le port 5000
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":5000 " ^| findstr "LISTENING"') do taskkill /F /PID %%p /T > nul 2>&1

echo Termine.

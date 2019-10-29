:: This script takes the following parameters:
:: 1 = Optional -prod to use the production server set in the environment
:: 2 = Optional path to migrations folder in CmsData
:: This script is intended to update all databases on a server, if you want to update a single database, run Migrate-Databases.ps1 with the -db parameter
@echo off
setlocal
set useprod=%1
set migrations=%2
if EXIST env.cmd call env.cmd
if "%migrations%" EQU "" set migrations=%~dp0CmsData\Migrations
set "h=10000"
if "%DatabaseMigrations%" EQU "" set DatabaseMigrations=0
for %%i IN (%DatabaseMigrations%) DO call :migration %%i
goto :EOF

:migration
cmd /c start powershell.exe .\Migrate-Databases.ps1 -start %1 -end %h% -scripts %migrations% %useprod%
set h=%1
:EOF
endlocal
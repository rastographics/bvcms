:: This script takes the following parameters:
:: 1 = Optional build configuration [Release|Debug|test] Release is the default
:: Optionally set %on_error% to the command you want to run if a step fails
@echo off
setlocal
set buildconfiguration=%1
if EXIST env.cmd call env.cmd
if "%on_error%" EQU "" set "on_error=exit 1"
if "%buildconfiguration%" EQU "" echo No build configuration specified - using Release && set buildconfiguration=Release
set "path=%path%;%ProgramFiles%\IIS\Microsoft Web Deploy V3;%ProgramFiles%\IIS\Microsoft Web Deploy V3"
set buildoutput=%~dp0build\Deploy\%buildconfiguration%\Files

for %%i IN (%webservers%) DO cmd /c deployserver.cmd %%i %site% %DeployUsername% %DeployPassword% %buildconfiguration%

if EXIST %USERPROFILE%\Documents\published%site%.ps1 powershell.exe %USERPROFILE%\Documents\published%site%.ps1 %buildconfiguration%
endlocal
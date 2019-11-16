:: This script takes the following parameters:
:: 1 - List of webservers to deploy to
:: 2 - IIS site name to deploy to
:: 3 - Administrator account name
:: 4 - Administrator account password
:: 5 - Optional build configuration [Release|Debug|test] Release is the default
:: Example: deploy 192.168.0.15,192.168.0.16 cms administrator adminpassword Release
:: Optionally set %on_error% to the command you want to run if a step fails
@echo off
setlocal
set webservers=%1
set site=%2
set DeployUsername=%3
set DeployPassword=%4
set buildconfiguration=%5
if EXIST env.cmd call env.cmd
if "%on_error%" EQU "" set "on_error=exit 1"
if "%webservers%" EQU "" echo "webservers parameter not set. Exiting..." & goto EOF
if "%site%" EQU "" echo "SITE parameter not set. Exiting..." & goto EOF
if "%DeployUsername%" EQU "" echo "DEPLOYUSERNAME parameter not set. Exiting..." & goto EOF
if "%DeployPassword%" EQU "" echo "DEPLOYPASSWORD parameter not set. Exiting..." & goto EOF
if "%buildconfiguration%" EQU "" echo No build configuration specified - using Release && set buildconfiguration=Release
set "path=%path%;%ProgramFiles%\IIS\Microsoft Web Deploy V3;%ProgramFiles%\IIS\Microsoft Web Deploy V3"
set buildoutput=%~dp0build\Deploy\%buildconfiguration%\Files

for %%i IN (%webservers%) DO cmd /c deployserver.cmd %%i %site% %DeployUsername% %DeployPassword% %buildconfiguration%

if EXIST %USERPROFILE%\Documents\published%site%.ps1 powershell.exe %USERPROFILE%\Documents\published%site%.ps1 -build %buildconfiguration%
:EOF
endlocal

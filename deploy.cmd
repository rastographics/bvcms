:: This script takes the following parameters:
:: 1 - build configuration [Release|Debug|test] Release is the default
:: 2 - List of webservers to deploy to
:: 3 - IIS site name to deploy to
:: 4 - Administrator account name
:: 5 - Administrator account password
:: Example: deploy Release 192.168.0.15,192.168.0.16 cms administrator adminpassword
:: Optionally set %on_error% to the command you want to run if a step fails
@echo off
setlocal
set buildconfiguration=%1
set webservers=%2
set site=%3
set DeployUsername=%4
set DeployPassword=%5
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

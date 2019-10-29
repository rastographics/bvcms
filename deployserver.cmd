:: This script is intended to be called from deploy.cmd but 
:: This script takes the following parameters:
:: 1 = The address of the server to deploy to
:: 2 = The name of the site to deploy to
:: 3 = The deployment user name
:: 4 = The deployment password
:: Optionally set the environment variables for %bringdown% and %bringup% to run a script on the server before and after deployment
:: Optionally set the environment variable for %setupscript% to run a setup script on the server after deployment
:: Optionally set %on_error% to the command you want to run if a step fails
@echo off
set dest=http://%1
set sitename=%2
set deployusername=%3
set deploypassword=%4
set buildconfiguration=%5
if "%sitename%" EQU "" echo No sitename specified && exit 1
if "%deployusername%" EQU "" echo No deployusername specified && exit 1
if "%deploypassword%" EQU "" echo No deploypassword specified && exit 1
if "%buildconfiguration%" EQU "" echo No build configuration specified - using Release && set buildconfiguration=Release
set buildoutput=%~dp0build\Deploy\%buildconfiguration%\Files

if NOT "%bringdown%" EQU "" (
    msdeploy -verb:sync -allowUntrusted -source:runCommand='%bringdown%',waitInterval=30000,waitAttempts=1 -dest:auto,computerName=%dest%/MsDeployAgentService,userName=%deployusername%,password=%deploypassword% || %on_error%
    timeout 15
)

msdeploy -verb:sync -allowUntrusted -source:contentPath=%buildoutput% -dest:contentPath=%sitename%,computerName=%dest%/MsDeployAgentService,userName=%deployusername%,password=%deploypassword% || %on_error%

if NOT "%setupscript%" EQU "" (
    msdeploy -verb:sync -allowUntrusted -source:runCommand=%setupscript% -dest:auto,computerName=%dest%/MsDeployAgentService,userName=%deployusername%,password=%deploypassword% || %on_error%
)

if NOT "%bringup%" EQU "" (
    timeout 2
    msdeploy -verb:sync -allowUntrusted -source:runCommand=%bringup% -dest:auto,computerName=%dest%/MsDeployAgentService,userName=%deployusername%,password=%deploypassword% || %on_error%
)
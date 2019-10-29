:: This script takes the following parameters:
:: 1 = Optional build configuration [Release|Debug|test] Release is the default
:: Optionally set %on_error% to the command you want to run if a step fails
@echo off
setlocal
set path=%path%;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin
set "source_root=%~dp0"
set buildconfiguration=%1
if EXIST env.cmd call env.cmd
if "%on_error%" EQU "" set "on_error=exit 1"
if "%buildconfiguration%" EQU "" echo No build configuration specified - using Release && set buildconfiguration=Release
if EXIST ..\private-gists\WebConfig copy /y ..\private-gists\WebConfig\*.config %source_root%CmsWeb

pushd %source_root%CmsTwilio
msbuild CmsTwilio.csproj /p:Platform=AnyCPU;Configuration=%buildconfiguration%;OutputPath=bin\%buildconfiguration% /clp:ErrorsOnly || %on_error%
popd

pushd %source_root%RepairOrg
msbuild RepairOrg.csproj /p:Platform=AnyCPU;Configuration=%buildconfiguration%;OutputPath=bin\%buildconfiguration% /clp:ErrorsOnly || %on_error%
popd

copy /y %source_root%CmsTwilio\bin\%buildconfiguration%\CmsTwilio.exe %source_root%CmsWeb\bin
copy /y %source_root%RepairOrg\bin\%buildconfiguration%\RepairOrg.exe %source_root%CmsWeb\bin

pushd %source_root%CmsWeb
call npm install
call gulp
msbuild CmsWeb.csproj /p:Platform=AnyCPU;Configuration=%buildconfiguration%;DeployOnBuild=true;PublishProfile=%buildconfiguration% /clp:ErrorsOnly || %on_error%
popd
endlocal
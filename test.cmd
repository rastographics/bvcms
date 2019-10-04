:: This script depends on PSTools being added to the PATH
:: https://docs.microsoft.com/en-us/sysinternals/downloads/pstools
setlocal
REG ADD HKCU\Software\Sysinternals\PsKill /v EulaAccepted /t REG_DWORD /d 1 /f
REG ADD HKCU\Software\Sysinternals\PsList /v EulaAccepted /t REG_DWORD /d 1 /f
nuget install Codecov -OutputDirectory packages || exit 1
nuget install OpenCover -OutputDirectory packages || exit 2
for /f %%f in ('dir /b packages\opencover.*') do set OpenCover=%~dp0packages\%%f\tools\opencover.console.exe
for /f %%f in ('dir /b packages\xunit.runner.console.*') do set xunit=%~dp0packages\%%f\tools\net461\xunit.console.x86.exe
for /f %%f in ('dir /b packages\codecov.*') do set codecov=%~dp0packages\%%f\tools\codecov.exe
for /f %%f in ('dir /b packages\selenium.webdriver.chromedriver.*') do set chromedriverdir=%~dp0packages\%%f\driver\win32\
set test_coverage=%~dp0test_coverage.xml
set opencover_filters="+[*]* -[*Tests]* -[xunit.*]*"
set iisexpress="%ProgramFiles%\IIS Express\iisexpress.exe"
echo using %OpenCover%
echo using %xunit%
echo using %codecov%
set target_tests=.\UnitTests\CMSDataTests\bin\Debug\CMSDataTests.dll
set target_tests=%target_tests% .\UnitTests\CMSWebTests\bin\Debug\CMSWebTests.dll
set target_tests=%target_tests% .\UnitTests\UtilityExtensionsTests\bin\Debug\UtilityExtensionsTests.dll
set integration_tests=.\IntegrationTests\bin\Debug\IntegrationTests.dll
echo quit | sqlcmd -S localhost -q "drop database cms_localhost"
echo quit | sqlcmd -S localhost -q "drop database cmsi_localhost"
%OpenCover% -register:user -target:"%xunit%" -targetargs:"%target_tests% -noshadow -teamcity" -filter:%opencover_filters% -output:"%test_coverage%" || exit 7
IF NOT EXIST %test_coverage% (
  echo File not found: %test_coverage%
  exit 8
)
IF "%CodeCovToken%" NEQ "" %codecov% -f "%test_coverage%" -t "%CodeCovToken%"
set "IISEXPRESS_HOST=%OpenCover%"
set placeholder="{0}"
set "IISEXPRESS_ARGS=-register:user -target:%iisexpress% -targetargs:%placeholder% -filter:%opencover_filters% -output:%test_coverage%"
del %test_coverage%
%OpenCover% -register:user -target:"%xunit%" -targetargs:"%integration_tests% -noshadow -teamcity" -filter:%opencover_filters% || exit 9

pskill -nobanner -t iisexpress.exe
:waitforopencover
@echo off
pslist -nobanner opencover.console >nul 2>&1
IF ERRORLEVEL 1 (
  goto opencoverexited
) ELSE (
  goto waitforopencover
)
:opencoverexited
@echo on

IF NOT EXIST %test_coverage% (
  echo File not found: %test_coverage%
  exit 10
)
IF "%CodeCovToken%" NEQ "" %codecov% -f "%test_coverage%" -t "%CodeCovToken%"
endlocal
setlocal
nuget install Codecov -OutputDirectory packages || exit 1
nuget install OpenCover -OutputDirectory packages || exit 2
for /f %%f in ('dir /b packages\opencover.*') do set OpenCover=%~dp0packages\%%f\tools\opencover.console.exe
for /f %%f in ('dir /b packages\xunit.runner.console.*') do set xunit=%~dp0packages\%%f\tools\net461\xunit.console.x86.exe
for /f %%f in ('dir /b packages\codecov.*') do set codecov=%~dp0packages\%%f\tools\codecov.exe
for /f %%f in ('dir /b packages\selenium.webdriver.chromedriver.*') do set chromedriverdir=%~dp0packages\%%f\driver\win32\
if "%PR%" EQU "" set PR=local
set root=%~dp0%
set opencover_filters="+[*]* -[*Tests]* -[xunit.*]*"
set iisexpress="%ProgramFiles%\IIS Express\iisexpress.exe"
echo using %OpenCover%
echo using %xunit%
echo using %codecov%
set "target_tests=.\UnitTests\CMSDataTests\bin\Debug\CMSDataTests.dll"
set "target_tests=%target_tests%,.\UnitTests\CmsSharedTests\bin\Debug\CmsSharedTests.dll"
set "target_tests=%target_tests%,.\UnitTests\CMSWebTests\bin\Debug\CMSWebTests.dll"
set "target_tests=%target_tests%,.\UnitTests\eSpaceClientTests\bin\Debug\eSpaceClientTests.dll"
set "target_tests=%target_tests%,.\UnitTests\ImageDataTests\bin\Debug\ImageDataTests.dll"
set "target_tests=%target_tests%,.\UnitTests\UtilityExtensionsTests\bin\Debug\UtilityExtensionsTests.dll"
::clean up
echo quit | sqlcmd -S (local) -q "IF DB_ID('CMS_localhost') IS NOT NULL ALTER DATABASE CMS_localhost SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE CMS_localhost"
echo quit | sqlcmd -S (local) -q "IF DB_ID('CMSi_localhost') IS NOT NULL ALTER DATABASE CMSi_localhost SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE CMSi_localhost"
::Unit tests
for %%i IN (%target_tests%) DO cmd /c %OpenCover% -register:user -target:"%xunit%" -targetargs:"%%i -noshadow -teamcity" -filter:%opencover_filters% -output:"%~dp0%%~ni.xml"
IF "%CodeCovToken%" NEQ "" (
  for %%i IN (%target_tests%) DO cmd /c %codecov% -f "%~dp0%%~ni.xml" --root %root% --pr %PR% --name "%%~ni" --flag unittests -t "%CodeCovToken%"
)
del /Q *Tests.xml
::Integration tests
set test_coverage=%~dp0IntegrationTests.xml
set "integration_tests=.\IntegrationTests\bin\Debug\IntegrationTests.dll"
set "IISEXPRESS_HOST=%OpenCover%"
set placeholder="{0}"
set "IISEXPRESS_ARGS=-register:user -target:%iisexpress% -targetargs:%placeholder% -filter:%opencover_filters% -output:%test_coverage%"
%OpenCover% -register:user -target:"%xunit%" -targetargs:"%integration_tests% -noshadow -teamcity" -filter:%opencover_filters% || exit 9

@echo off
taskkill /F /IM iisexpress.exe
:waitforopencover
tasklist /FI "IMAGENAME eq opencover.console.exe" 2>NUL | find /I /N "No tasks">NUL
IF ERRORLEVEL 1 (
  goto waitforopencover
) ELSE (
  goto opencoverexited
)
:opencoverexited
@echo on

IF NOT EXIST %test_coverage% (
  echo File not found: %test_coverage%
  exit 10
)
IF "%CodeCovToken%" NEQ "" %codecov% -f "%test_coverage%" --root %root% --pr %PR% --name "IntegrationTests" --flag integrationtests -t "%CodeCovToken%"
endlocal

@echo OFF
echo Creating a 'GeneratedReports' folder if it does not exist...
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"
 
echo Removing any previous test execution files to prevent issues overwriting...
IF EXIST "%~dp0AutomatedSimTemplateTests.trx" del "%~dp0AutomatedSimTemplateTests.trx%"
 
echo Removing any previously created test output directories...
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"
 
echo Running the tests against the targeted output...
call :RunOpenCoverUnitTestMetrics
 
echo Generating the report output based on the test results...
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)
 
REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%
 
:RunOpenCoverUnitTestMetrics
"%~dp0..\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%VS140COMNTOOLS%\..\IDE\mstest.exe" ^
-targetargs:"/testcontainer:\"%~dp0AutomatedSimTemplateTests\bin\Debug\AutomatedSimTemplateTests.dll\" /resultsfile:\"%~dp0AutomatedSimTemplateTests.trx\"" ^
-filter:"+[SimTemplate*]* -[SimTemplate.*Tests]* -[*]SimTemplate.RouteConfig" ^
-mergebyhash ^
-skipautoprops ^
-output:"%~dp0GeneratedReports\AutomatedSimTemplateTests.xml"
exit /b %errorlevel%
 
:RunReportGeneratorOutput
"%~dp0..\packages\ReportGenerator.2.4.5.0\tools\ReportGenerator.exe" ^
-reports:"%~dp0GeneratedReports\AutomatedSimTemplateTests.xml" ^
-targetdir:"%~dp0GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%
 
:RunLaunchReport
start "report" "%~dp0GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%
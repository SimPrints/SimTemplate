# SimTemplate
A WPF application to facilitate manual construction of templates for captures in the SimPrints [DatastoreAPI](https://github.com/SimPrints/DataStoreAPI) database.

## Setup
### Installer
The Visual Studio solution includes a [WiX toolset](http://wixtoolset.org/) project **SimTemplateInstaller**, that Visual Studio will not recognise unless the toolset is installed. There is a [NuGet extension](https://visualstudiogallery.msdn.microsoft.com/b6868002-9770-4479-80a7-259de34df527?SRC=VSIDE) that installs this functionality.
Once installed, this project may be built to create an .msi installer that will install the production code onto a PC at:
``
<Program_Files>\SimPrints\SimTemplate
``
### User Settings
The application requires two user settings to be set in order to run: RootUrl and ApiKey. If these settings are missing when the application starts, it will prompt the user to enter them.

The RoolUrl setting should point to the root URL for the SimPrints DatastoreAPI. The ApiKey setting should be the GUID token that grants access to the API. Both should be obtained from [SimPrints](https://github.com/SimPrints).

The settings will persist between application launches and are associated with a user account.

## Documentation (for developers)
### Overview
The SimTemplate application utilises the [Model-View-ViewModel Pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel), where the ViewModel's behaviour is determined by a state machine implemented using the [State Pattern](https://en.wikipedia.org/wiki/State_pattern).

### Help Files
The code is (/will be!) self-documenting using the [Sandcastle Help File Builder (SHFB)](https://github.com/EWSoftware/SHFB) tool.
The documentation can be generated from the **SimTemplateDocs** project using the SHFB GUI, or using MSBuild with the following command:
```
MSBuild.exe SimTemplateDocs.shfbproj
```
The [Building Projects Outside the GUI](http://ewsoftware.github.io/SHFB/html/8ffc0d37-0215-4609-b6f8-dba53a6c5063.htm) help topic can provide further information on building the documentation without needing to install SHFB.

### Logging
The application (and tests) logs using the log4net library to both a static file and over UDP.

The static file can be found at:
``
C:\ProgramData\SimTemplate\Logs\logging.log
``
The UDP logs are configured to be sent to:

* **Address:** localhost or 127.0.0.2
* **Port:** 7071

Tools such as the excellet [Log4View](http://www.log4view.com/log4view/) can be used to visualise the UDP logs in real-time.

### Testing
Automated tests can be found in the UnitTests/**AutomatedSimTemplateTests** project. Code coverage analysis can be run using UnitTests/RunCodeCoverage.bat and the resulting (including a [summary for Github](http://htmlpreview.github.com/?https://github.com/SimPrints/SimTemplate/blob/master/UnitTests/GeneratedReports/ReportGenerator%20Output/summary.htm)) can be found at UnitTests/GeneratedReports.

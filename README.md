# SimTemplate
A WPF application to facilitate manual construction of templates for captures in the SimPrints database.

## Documentation
### Overview
The SimTemplate application utilises the [Model-View-ViewModel Pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel), where the ViewModel's behaviour is determined by a state machine implemented using the [State Pattern](https://en.wikipedia.org/wiki/State_pattern).

### Help Files
The code is(/will be!) self-documenting using the [Sandcastle Help File Builder (SHFB)](https://github.com/EWSoftware/SHFB) tool.
The documentation can be generated using the SHFB GUI, or using MSBuild with the following command:
```
MSBuild.exe HelpFile.shfbproj
```
The [Building Projects Outside the GUI](http://ewsoftware.github.io/SHFB/html/8ffc0d37-0215-4609-b6f8-dba53a6c5063.htm) help topic can provide further information on building the documentation.

### Logging
The application logs using the log4net library to both a static file and over UDP.

The static file can be found at:

``C:\ProgramData\SimTemplate\Logs\logging.log``

The UDP logs are configured to be sent to:

* **Address:** localhost or 127.0.0.2
* **Port:** 7071

Tools such as the excellet [Log4View](http://www.log4view.com/log4view/) can be used to visualise the UDP logs in real-time.

## TODOs :date:
- Specify scanner in ReST request as soon as API accepts this parameter
- User guidance
  * Help icon
  * Provide a How To
- Image rotation?
- Convert application to asynchronous MVVM
  * See [MSDN blog posts](https://msdn.microsoft.com/en-us/magazine/mt149362?author=Stephen+Cleary)
  * Low priority but a good exercise! All Windows Store apps must 

Below 5 settings are required to be added in app settings:
1. RootLogFolderPath		Root Folder Path where log files would be created
This is mandatory and must be added to use file logger 
2. RootAppName				Root App Name 
This is non mandatory. If not added then UTO would be taken as a default RootAppName
3. AppId					Application Id 
This is non mandatory. If not set in app settings then must be set while calling constructor or call method overloads which allows to set this setting at run time.
If not set and set in constructor then app setting would be replaced by that. Same for overloaded method.
4. AppName					Application Name
This is non mandatory. If not set in app settings then must be set while calling constructor or call method overloads which allows to set this setting at run time.
If not set and set in constructor then app setting would be replaced by that. Same for overloaded method.
5. App.LogLevel				Log Levels to be enabled 
Below 7 levels are available Trace, Debug, Information, Warning, Error, Critical, None
Default value would be Error and above for Production Environment.
Default value would be Trace and above for Development Environment.
Read following article for more details about log levels
https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-3.0
public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

Below is the sample for settings:
<add key="RootLogFolderPath" value="D:\"/>
<add key="RootAppName" value="UTO"/>
<add key="AppId" value="1"/>
<add key="AppName" value="TestApp1"/>
<add key="App.LogLevel" value="Information"/>


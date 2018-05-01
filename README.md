# Serilog.Sinks.Splunk

[![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk) ![NuGet Version](https://buildstats.info/nuget/Serilog.Sinks.Splunk) 
 [![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog)
 
A Serilog sink that writes events to the [Splunk](https://splunk.com). Supports .NET 4.5+, .NET Core, and platforms compatible with the [.NET Platform Standard](https://github.com/dotnet/corefx/blob/master/Documentation/architecture/net-platform-standard.md) 1.1 including Windows 8 & UWP, Windows Phone and Xamarin.

[![Package Logo](https://serilog.net/images/serilog-sink-nuget.png)](https://nuget.org/packages/serilog.sinks.splunk)

**Package** - [Serilog.Sinks.Splunk](https://nuget.org/packages/serilog.sinks.splunk)

## Getting started

To get started install the *Serilog.Sinks.Splunk* package:

```powershell
PM> Install-Package Serilog.Sinks.Splunk
```

OR

```bash
> dotnet add package Serilog.Sinks.Splunk
```

Using the Event Collector (Splunk 6.3 and above)

```csharp
var log = new LoggerConfiguration()
     .WriteTo.EventCollector("https://mysplunk:8088/services/collector", "myeventcollectortoken")
    .CreateLogger();
```

More information is available on the [wiki](https://github.com/serilog/serilog-sinks-splunk/wiki).

### Build status

Branch  | AppVeyor | Travis
------------- | ------------- |-------------
master |  [![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk/branch/master) | ![](https://travis-ci.org/serilog/serilog-sinks-splunk.svg?branch=master) 
dev | [![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al/branch/dev?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk/branch/dev) | ![](https://travis-ci.org/serilog/serilog-sinks-splunk.svg?branch=dev)

_Serilog is copyright &copy; 2013-2016 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html). Needle and thread logo a derivative of work by [Kenneth Appiah](http://www.kensets.com/)._

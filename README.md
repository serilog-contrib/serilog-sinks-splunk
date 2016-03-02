# serilog-sinks-splunk
[![Package Logo](http://serilog.net/images/serilog-sink-nuget.png)](http://nuget.org/packages/serilog.sinks.splunk)

[![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk) 
[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Splunk.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Splunk/)

A sink for Serilog that writes events to [Splunk](https://splunk.com). Moved from the [main Serilog repository](https://github.com/serilog/serilog) for independent versioning. Published to [NuGet](http://www.nuget.org/packages/serilog.sinks.splunk).

**Package** - [Serilog.Sinks.Splunk](http://nuget.org/packages/serilog.sinks.splunk)
| **Platforms** - .NET 4.5+, PCL

## Getting started

To get started install the *Serilog.Sinks.Splunk* package from Visual Studio's *NuGet* console:

```powershell
PM> Install-Package Serilog.Sinks.Splunk
```

Using the new Event Collector in Splunk 6.3

```csharp 
var log = new LoggerConfiguration()
     .WriteTo.EventCollector("https://mysplunk:8088/services/collector", "myeventcollectortoken")
    .CreateLogger();
```
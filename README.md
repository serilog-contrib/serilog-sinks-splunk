# serilog-sinks-splunk
[![Package Logo](http://serilog.net/images/serilog-sink-nuget.png)](http://nuget.org/packages/serilog.sinks.splunk)

[![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk) 
[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Splunk.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Splunk/)

A sink for Serilog that writes events to [Splunk](https://splunk.com). Moved from the [main Serilog repository](https://github.com/serilog/serilog) for independent versioning. Published to [NuGet](http://www.nuget.org/packages/serilog.sinks.splunk).

**Package** - [Serilog.Sink.Splunk](http://nuget.org/packages/serilog.sink.splunk)
| **Platforms** - .NET 4.5+, PCL

## Getting started

To get started install the *Serilog.Sinks.Seq* package from Visual Studio's *NuGet* console:

```powershell
PM> Install-Package Serilog.Sinks.Splunk
```

Set up to log via TCP

```csharp
var log = new LoggerConfiguration()
    .WriteTo.SplunkViaTcp("127.0.0.1", 10001)
    .CreateLogger();
```

Or maybe UDP

```csharp
var log = new LoggerConfiguration()
    .WriteTo.SplunkViaUdp("127.0.0.1", 10000)
    .CreateLogger();
```

Or maybe HTTP

```csharp
var generalSplunkContext = new Context(Scheme.Https, "127.0.0.1", 8089);

var transmitterArgs = new TransmitterArgs
{
    Source = "Splunk.Sample",
    SourceType = "Splunk Sample Source"
};

const string username = "my splunk user";
const string password = "my splunk password";
const string splunkIndex = "mysplunktest";

var serilogContext = new SplunkContext(generalSplunkContext, splunkIndex, username, password, null, transmitterArgs);

var log = new LoggerConfiguration()
     .WriteTo.SplunkViaHttp(serilogContext, 10, TimeSpan.FromSeconds(5))
    .CreateLogger();
```

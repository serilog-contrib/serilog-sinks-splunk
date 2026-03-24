# Serilog.Sinks.Splunk

![Build Status](https://github.com/serilog-contrib/serilog-sinks-splunk/actions/workflows/ci.yml/badge.svg?branch=dev)
[![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.Splunk.svg)](https://www.nuget.org/packages/Serilog.Sinks.Splunk)
[![GitHub Discussions](https://img.shields.io/github/discussions/serilog-contrib/serilog-sinks-splunk)](https://github.com/serilog-contrib/serilog-sinks-splunk/discussions)
 
A Serilog sink that writes events to the [Splunk](https://splunk.com). Supports .NET 8+, .NET 9, .NET 10, and platforms compatible with the [.NET Platform Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) `netstandard2.0`, `netstandard2.1`.

[![Package Logo](https://serilog.net/images/serilog-sink-nuget.png)](https://nuget.org/packages/serilog.sinks.splunk)

**Package** - [Serilog.Sinks.Splunk](https://nuget.org/packages/serilog.sinks.splunk)

## Getting started

To get started install the *Serilog.Sinks.Splunk* package:

```powershell
PM> Install-Package Serilog.Sinks.Splunk
```

OR

```bash
$ dotnet add package Serilog.Sinks.Splunk
```

If using the `TCP` or `UDP` sinks install the following packages

* TCP: `Serilog.Sinks.Splunk.TCP`
* UDP: `Serilog.Sinks.Splunk.UDP`

To start using the Splunk Event Collector (Splunk 6.3 and above), logging can be setup as follows.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.EventCollector("https://mysplunk:8088/services/collector/event", "myeventcollectortoken")
    .CreateLogger();
```

If using `appsettings.json` for configuration the following example illustrates using the Event Collector and Console sinks.

```javascript
{
    "Serilog": {
        "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.Splunk"],
        "MinimumLevel": "Information",
        "WriteTo": [{
                "Name": "Console"
            },
            {
                "Name": "EventCollector",
                "Args": {
                    "splunkHost": "http://splunk:8088",
                    "uriPath": "services/collector/event",
                    "eventCollectorToken": "00112233-4455-6677-8899-AABBCCDDEEFF"
                }
            }
        ],
        "Properties": {
            "Application": "Serilog Splunk Console Sample"
        }
    }
}
```

### Automatic host name

Use the `includeHost` option to automatically set the Splunk `host` metadata field to the machine name. This is opt-in and defaults to `false`.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.EventCollector(
        "https://mysplunk:8088",
        "myeventcollectortoken",
        includeHost: true)
    .CreateLogger();
```

Or via `appsettings.json`:

```javascript
{
    "Name": "EventCollector",
    "Args": {
        "splunkHost": "http://splunk:8088",
        "eventCollectorToken": "00112233-4455-6677-8899-AABBCCDDEEFF",
        "includeHost": true
    }
}
```

If `host` is explicitly set, it takes precedence over `includeHost`.

More information about Serilog is available on the [wiki](https://github.com/serilog/serilog-sinks-splunk/wiki).

_Serilog is copyright &copy; 2013-2026 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html). Needle and thread logo a derivative of work by [Kenneth Appiah](http://www.kensets.com/)._

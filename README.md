# serilog-sinks-splunk

[![Build status](https://ci.appveyor.com/api/projects/status/yt40wg34t8oj61al?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-splunk)

A sink for Serilog that writes events to [Splunk](https://splunk.com). Moved from the [main Serilog repository](https://github.com/serilog/serilog) for independent versioning. Published to [NuGet](http://www.nuget.org/packages/serilog.sinks.splunk).

**Package** - [Serilog.Sink.Splunk](http://nuget.org/packages/serilog.sink.splunk)
| **Platforms** - .NET 4.5+, PCL

```csharp
var log = new LoggerConfiguration()
    .WriteTo.SplunkViaUdp("127.0.0.1", 10000)
    .WriteTo.SplunkViaTcp("127.0.0.1", 10001)
    .CreateLogger();
```

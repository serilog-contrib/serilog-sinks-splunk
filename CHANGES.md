## 5.2.0
- Add .NET 10 support
- Migrate samples to .NET Aspire
- Update CI to GitHub Actions with .NET 8/9/10 SDKs
- Bump Serilog to 4.2.0
- [#165](https://github.com/serilog-contrib/serilog-sinks-splunk/issues/165) Add `includeHost` option to automatically set the host metadata field from the machine name
- [#159](https://github.com/serilog-contrib/serilog-sinks-splunk/issues/159) Add default constructor to `CustomField` for configuration deserialization
- Fix JSON injection in custom fields serialization
- Add `IDisposable` to `EventCollectorSink` for proper HTTP resource cleanup
- Add `IDisposable` to `UdpSink` for proper socket cleanup
- Fix URL path detection in `EventCollectorRequest` to avoid false hostname matches
- Fix `FixedSizeQueue.Count` return type from `decimal` to `int`
- Enable `TreatWarningsAsErrors` on TCP and UDP projects

## 5.1.0
- Add .NET 9.0 support
- Bump Serilog from 4.0.2 to 4.1.0
- Dependency updates for Microsoft.Extensions.Hosting and Configuration

## 5.0.1
- [#189](https://github.com/serilog/serilog-sinks-splunk/pull/189) Fix PackageIconUrl deprecation
- [#184](https://github.com/serilog/serilog-sinks-splunk/pull/184) UDP and TCP sink fixes for [#155](https://github.com/serilog/serilog-sinks-splunk/issues/155)

## 5.0.0
- [#185](https://github.com/serilog/serilog-sinks-splunk/pull/185) Fix for no logs issue [#183](https://github.com/serilog/serilog-sinks-splunk/issues/183)
- [#181](https://github.com/serilog/serilog-sinks-splunk/pull/181) Updates for UDP and TCP sinks
- [#186](https://github.com/serilog/serilog-sinks-splunk/pull/186) Common props for version and NuGet package info

## 4.0.0
- *BREAKING* Upgrade to Serilog 4.0.0 and Serilog.Sinks.PeriodicBatching 4.x using `IBatchedLogEventSink`
- [#211](https://github.com/serilog/serilog-sinks-splunk/pull/211) Fix for [#205](https://github.com/serilog/serilog-sinks-splunk/issues/205) - allow collector path in host URL
- [#172](https://github.com/serilog/serilog-sinks-splunk/pull/172) Sub-second decimal precision for timestamps
- [#171](https://github.com/serilog/serilog-sinks-splunk/pull/171) .NET 8.0 updates
- [#166](https://github.com/serilog/serilog-sinks-splunk/pull/166) Splunk 9.1 and .NET 6 support
- [#174](https://github.com/serilog/serilog-sinks-splunk/pull/174) Migrate CI to GitHub Actions
- Add TraceId and SpanId to JSON output
- Add code coverage reporting
- Drop `netstandard1.1` and `net45` target frameworks

## 3.6.0
- [#138](https://github.com/serilog/serilog-sinks-splunk/pull/138)

## 3.5.0
- [#134](https://github.com/serilog/serilog-sinks-splunk/pull/134)

## 3.4.0
- [#126](https://github.com/serilog/serilog-sinks-splunk/pull/126)
- [#122](https://github.com/serilog/serilog-sinks-splunk/pull/122)
- [#121](https://github.com/serilog/serilog-sinks-splunk/pull/121)

## 3.3.0
- Correct issues relating to #76 and signing.
- Bump version to 3.3 for core Sink.
- Bump version to 1.2 for UDP Sink.
- Bump version to 1.2 for TCP Sink.

## 3.2.0
- Remove TravisCI for Linux builds
- Add AppVeyor for Linux Builds
- [#76](https://github.com/serilog/serilog-sinks-splunk/issues/76)

## 3.1.0
- [#105](https://github.com/serilog/serilog-sinks-splunk/pull/105)

## 3.0.0
- [#76](https://github.com/serilog/serilog-sinks-splunk/issues/76) Add strong naming/signing to `Serilog.Sinks.Splunk`.
- [#88](https://github.com/serilog/serilog-sinks-splunk/issues/88) Split Sinks into separate packages for maintainability.
- *NOTE* Breaking changes.  TCP & UDP Sinks moved to new packages
    - Serilog.Sinks.Splunk (3.0.x)
    - Serilog.Sinks.Splunk.TCP (1.0.x)
    - Serilog.Sinks.Splunk.UDP (1.0.x)

## 2.5.0
- [#78](https://github.com/serilog/serilog-sinks-splunk/issues/78) Update `System.Net.Http` references to match other similar sinks.
- [#79](https://github.com/serilog/serilog-sinks-splunk/issues/79) Addition of optional `LoggingLevelSwitch` param to EventCollector sink.

## 2.4.0
- [#62](https://github.com/serilog/serilog-sinks-splunk/issues/62) Default fields added by Serilog to splunk
- [#63](https://github.com/serilog/serilog-sinks-splunk/issues/63) Possible thread leak when ILogger instances are disposed

## 2.3.0
- [#59](https://github.com/serilog/serilog-sinks-splunk/issues/59) Added ability to use custom fields with HEC.  See http://dev.splunk.com/view/event-collector/SP-CAAAFB6.

## 2.2.1
- [#47](https://github.com/serilog/serilog-sinks-splunk/issues/47) Tooling updates to VS2017
- [#48](https://github.com/serilog/serilog-sinks-splunk/issues/48)
- [#49](https://github.com/serilog/serilog-sinks-splunk/issues/49)
- [#52](https://github.com/serilog/serilog-sinks-splunk/issues/52)

## 2.1.3
- [#45](https://github.com/serilog/serilog-sinks-splunk/issues/45) - Deadlock fix on UI thread.

## 2.1.2
- [#43](https://github.com/serilog/serilog-sinks-splunk/issues/43) - Extend sink & static configuration to allow for custom JSON formatter.

## 2.1.1
- [#38](https://github.com/serilog/serilog-sinks-splunk/issues/38) - Fix for HttpEventlogCollector and sourceType
- Clean up of sample app using examples of host, sourcetype, source override

## 2.1.0

* Change to use a standalone formatter
* Resolves - [#32](https://github.com/serilog/serilog-sinks-splunk/issues/32) & - [#26](https://github.com/serilog/serilog-sinks-splunk/issues/26) by exposing `HttpMessageHandler`
* Resolves - [#30](https://github.com/serilog/serilog-sinks-splunk/issues/30) by ignoring OSX build and including tests in `build.sh` for TravisCI

## 2.0
 - Support for DotNet Core
 - Event Collector fluent interface changed to `.WriteTo.EventCollector`
 - Event Collector Sink targeting core
 - TCP/UDP Sinks targeting 4.5 *ONLY*
 - Updated Event Collector HTTP Client to add URI endpoint to host: "services/collector/event" if not included.
 - Event Collector changed to use epoch time [#15](https://github.com/serilog/serilog-sinks-splunk/pull/15)

## 1.8
 - Event Collector changed to use epoch time [#15](https://github.com/serilog/serilog-sinks-splunk/pull/15)

## 1.7
 - Better support for formatting including [#578](https://github.com/serilog/serilog/issues/578)
 - Cleanup on Event Collector

## 1.6.50
 - Streaming support for Event Collector
 
## 1.6.42
 - Added support for Splunk 6.3 Event Collector
 - Deprecated Splunk HTTP Sink using Management Port/API

## 1.5.30
 - Added switch for template rendering
 
## 1.5.0
 - Moved the sink from its [original location](https://github.com/serilog/serilog)

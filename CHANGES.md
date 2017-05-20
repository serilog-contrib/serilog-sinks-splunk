##2.2.0
- [#47] Tooling updates to VS2017

##2.1.2
- [#43](https://github.com/serilog/serilog-sinks-splunk/pull/43) - Extend sink & static configuration to allow for custom JSON formatter.

##2.1.1
- [#38](https://github.com/serilog/serilog-sinks-splunk/issues/38) - Fix for HttpEventlogCollector and sourceType
- Clean up of sample app using examples of host, sourcetype, source override

##2.1.0

* Change to use a standalone formatter
* Resolves #32 & #26 by exposing `HttpMessageHandler`
* Resolves #30 by ignoring OSX build and including tests in `build.sh` for TravisCI

##2.0
 - Support for DotNet Core
 - Event Collector fluent interface changed to `.WriteTo.EventCollector`
 - Event Collector Sink targeting core
 - TCP/UDP Sinks targeting 4.5 *ONLY*
 - Updated Event Collector HTTP Client to add URI endpoint to host: "services/collector" if not included.
 - Event Collector changed to use epoch time [#15](https://github.com/serilog/serilog-sinks-splunk/pull/15)

##1.8
 - Event Collector changed to use epoch time [#15](https://github.com/serilog/serilog-sinks-splunk/pull/15)

##1.7
 - Better support for formatting including [#578](https://github.com/serilog/serilog/issues/578)
 - Cleanup on Event Collector

##1.6.50
 - Streaming support for Event Collector
 
##1.6.42
 - Added support for Splunk 6.3 Event Collector
 - Deprecated Splunk HTTP Sink using Management Port/API

##1.5.30
 - Added switch for template rendering
 
 ##1.5.0
 - Moved the sink from its [original location](https://github.com/serilog/serilog)

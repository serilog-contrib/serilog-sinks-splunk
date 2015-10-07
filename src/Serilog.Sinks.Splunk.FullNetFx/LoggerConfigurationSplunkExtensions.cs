// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Splunk; 

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.SplunkViaHttp() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationSplunkExtensions
    {


        internal const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="splunkHost"></param>
        /// <param name="eventCollectorToken"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="renderTemplate"></param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaEventCollector(
            this LoggerSinkConfiguration sinkConfiguration,
            string splunkHost,
            string eventCollectorToken,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null, bool renderTemplate = true)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new SplunkViaEventCollectorSink(splunkHost, eventCollectorToken), restrictedToMinimumLevel);
        }


        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via http.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="host">The Splunk host that is configured for UDP logging</param>
        /// <param name="port">The UDP port</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template will be rendered</param>
        /// <returns></returns>
        /// <remarks>TODO: Add link to splunk configuration and wiki</remarks>
        public static LoggerConfiguration SplunkViaUdp(
            this LoggerSinkConfiguration loggerConfiguration,
            string host,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true)
        {
            var sink = new SplunkViaUdpSink(host, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }


        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via UDP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="hostAddresss">The Splunk host that is configured for UDP logging</param>
        /// <param name="port">The UDP port</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template is rendered</param>
        /// <returns>The logger configuration</returns>
        /// <remarks>TODO: Add link to splunk configuration and wiki</remarks>
        public static LoggerConfiguration SplunkViaUdp(
            this LoggerSinkConfiguration loggerConfiguration,
            IPAddress hostAddresss,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true)
        {
            var sink = new SplunkViaUdpSink(hostAddresss, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="hostAddresss">The Splunk host that is configured for UDP logging</param>
        /// <param name="port">The TCP port</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the message template is rendered</param>
        /// <returns></returns>
        /// <remarks>TODO: Add link to splunk configuration and wiki</remarks>
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            IPAddress hostAddresss,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
             bool renderTemplate = true)
        {
            var sink = new SplunkViaTcpSink(hostAddresss, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="host">The Splunk host that is configured for UDP logging</param>
        /// <param name="port">The TCP port</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template is rendered</param>
        /// <returns></returns>
        /// <remarks>TODO: Add link to splunk configuration and wiki</remarks>
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            string host,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
             bool renderTemplate = true)
        {
            var sink = new SplunkViaTcpSink(host, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

    }
}
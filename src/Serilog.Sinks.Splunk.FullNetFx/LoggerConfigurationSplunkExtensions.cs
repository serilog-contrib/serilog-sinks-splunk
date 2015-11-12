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
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.Splunk; 

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.SplunkViaEventCollector() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationSplunkExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via UDP.
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
            var sink = new UdpSink(host, port, formatProvider, renderTemplate);

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
            var sink = new UdpSink(hostAddresss, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="connectionInfo"></param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the message template is rendered</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            SplunkTcpSinkConnectionInfo connectionInfo,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true)
        {
            var sink = new TcpSink(connectionInfo, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="connectionInfo"></param>
        /// <param name="formatter">Custom formatter to use if you e.g. do not want to use the JsonFormatter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            SplunkTcpSinkConnectionInfo connectionInfo,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var sink = new TcpSink(connectionInfo, formatter);

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
        [Obsolete("Use the overload accepting a connection info object instead. This overload will be removed.", false)]
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            IPAddress hostAddresss,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
             bool renderTemplate = true)
        {
            var sink = new TcpSink(hostAddresss, port, formatProvider, renderTemplate);

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
        [Obsolete("Use the overload accepting a connection info object instead. This overload will be removed.", false)]
        public static LoggerConfiguration SplunkViaTcp(
            this LoggerSinkConfiguration loggerConfiguration,
            string host,
            int port,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
             bool renderTemplate = true)
        {
            var sink = new TcpSink(host, port, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
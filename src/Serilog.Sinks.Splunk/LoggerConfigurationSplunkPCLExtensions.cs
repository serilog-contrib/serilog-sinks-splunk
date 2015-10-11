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
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Splunk;

namespace Serilog
{
    /// <summary>
    /// Fluent extenstions for the Serilog configuration
    /// </summary>
    public static class LoggerConfigurationSplunkPCLExtensions
    {

        internal const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via UDP.
        /// </summary>
        /// <param name="configuration">The logger config</param>
        /// <param name="splunkHost">The Splunk host that is configured with an Event Collector</param>
        /// <param name="eventCollectorToken">The token provided to authenticate to the Splunk Event Collector</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="outputTemplate">The output template to be used when logging</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template will be rendered</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that the queue should be instpected for batching</param>
        /// <param name="batchSizeLimit">The size of the batch</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaEventCollector(
            this LoggerSinkConfiguration configuration,
            string splunkHost,
            string eventCollectorToken,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null, 
            bool renderTemplate = true,
            int batchIntervalInSeconds = 2,
            int batchSizeLimit = 10)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            
            var eventCollectorSink = new EventCollectorSink(
                splunkHost, 
                eventCollectorToken,
                batchIntervalInSeconds,
                batchSizeLimit,
                formatProvider,
                renderTemplate);

            return configuration.Sink(eventCollectorSink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via UDP.
        /// </summary>
        /// <param name="configuration">The logger config</param>
        /// <param name="splunkHost">The Splunk host that is configured with an Event Collector</param>
        /// <param name="eventCollectorToken">The token provided to authenticate to the Splunk Event Collector</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="outputTemplate">The output template to be used when logging</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template will be rendered</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that the queue should be instpected for batching</param>
        /// <param name="batchSizeLimit">The size of the batch</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaEventCollector(
            this LoggerSinkConfiguration configuration,
            string splunkHost,
            string eventCollectorToken,
            string source,
            string sourceType,
            string host,
            string index,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            int batchIntervalInSeconds = 2,
            int batchSizeLimit = 10)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));

            var eventCollectorSink = new EventCollectorSink(
                splunkHost,
                eventCollectorToken,
                source,
                sourceType,
                host,
                index,
                batchIntervalInSeconds,
                batchSizeLimit,
                formatProvider,
                renderTemplate);

            return configuration.Sink(eventCollectorSink, restrictedToMinimumLevel);
        }
    }
}
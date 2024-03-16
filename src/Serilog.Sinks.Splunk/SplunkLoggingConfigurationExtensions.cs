// Copyright 2018 Serilog Contributors
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


using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.Splunk;
using System;
using System.Net.Http;

namespace Serilog
{
    /// <summary>
    ///     Fluent configuration methods for Logger configuration
    /// </summary>
    public static class SplunkLoggingConfigurationExtensions
    {
        /// <summary>
        ///     Adds a sink that writes log events as to a Splunk instance via the HTTP Event Collector.
        /// </summary>
        /// <param name="configuration">The logger config</param>
        /// <param name="splunkHost">The Splunk host that is configured with an Event Collector</param>
        /// <param name="eventCollectorToken">The token provided to authenticate to the Splunk Event Collector</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the message template will be rendered</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter from output JSON message.</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that the queue should be instpected for batching</param>
        /// <param name="batchSizeLimit">The size of the batch</param>
        /// <param name="queueLimit">Maximum number of events in the queue</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <param name="subSecondDecimals">Timestamp sub-second precision</param>
        /// <returns></returns>
        public static LoggerConfiguration EventCollector(
            this LoggerSinkConfiguration configuration,
            string splunkHost,
            string eventCollectorToken,
            string uriPath = ConfigurationDefaults.DefaultEventCollectorPath,
            string source = ConfigurationDefaults.DefaultSource,
            string sourceType = ConfigurationDefaults.DefaultSourceType,
            string host = ConfigurationDefaults.DefaultHost,
            string index = ConfigurationDefaults.DefaultIndex,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            bool renderMessage = true,
            int batchIntervalInSeconds = 2,
            int batchSizeLimit = 100,
            int? queueLimit = null,
            HttpMessageHandler messageHandler = null,
            LoggingLevelSwitch levelSwitch = null,
            int subSecondDecimals = 3)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchSizeLimit,
                Period = TimeSpan.FromSeconds(batchIntervalInSeconds),
                EagerlyEmitFirstEvent = true,
                QueueLimit = queueLimit
            };

            var eventCollectorSink = new EventCollectorSink(
                splunkHost,
                eventCollectorToken,
                uriPath,
                source,
                sourceType,
                host,
                index,
                formatProvider,
                renderTemplate,
                renderMessage,
                subSecondDecimals: subSecondDecimals);

            var batchingSink = new PeriodicBatchingSink(eventCollectorSink, batchingOptions);

            return configuration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        ///     Adds a sink that writes log events as to a Splunk instance via the HTTP Event Collector.
        /// </summary>
        /// <param name="configuration">The logger config</param>
        /// <param name="splunkHost">The Splunk host that is configured with an Event Collector</param>
        /// <param name="eventCollectorToken">The token provided to authenticate to the Splunk Event Collector</param>
        /// <param name="jsonFormatter">The text formatter used to render log events into a JSON format for consumption by Splunk</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that the queue should be instpected for batching</param>
        /// <param name="batchSizeLimit">The size of the batch</param>
        /// <param name="queueLimit">Maximum number of events in the queue</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <returns></returns>
        public static LoggerConfiguration EventCollector(
            this LoggerSinkConfiguration configuration,
            string splunkHost,
            string eventCollectorToken,
            ITextFormatter jsonFormatter,
            string uriPath = ConfigurationDefaults.DefaultEventCollectorPath,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchIntervalInSeconds = 2,
            int batchSizeLimit = 100,
            int? queueLimit = null,
            HttpMessageHandler messageHandler = null,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (jsonFormatter == null) throw new ArgumentNullException(nameof(jsonFormatter));


            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchSizeLimit,
                Period = TimeSpan.FromSeconds(batchIntervalInSeconds),
                EagerlyEmitFirstEvent = true,
                QueueLimit = queueLimit
            };

            var eventCollectorSink = new EventCollectorSink(
                splunkHost,
                eventCollectorToken,
                uriPath,
                jsonFormatter,
                messageHandler);

            var batchingSink = new PeriodicBatchingSink(eventCollectorSink, batchingOptions);

            return configuration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
        }


        /// <summary>
        ///     Adds a sink that writes log events as to a Splunk instance via the HTTP Event Collector.
        /// </summary>
        /// <param name="configuration">The logger config</param>
        /// <param name="splunkHost">The Splunk host that is configured with an Event Collector</param>
        /// <param name="eventCollectorToken">The token provided to authenticate to the Splunk Event Collector</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If ture, the message template will be rendered</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that the queue should be instpected for batching</param>
        /// <param name="batchSizeLimit">The size of the batch</param>
        /// <param name="queueLimit">Maximum number of events in the queue</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <param name="fields">Customfields that will be indexed in splunk with this event</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter from output JSON message.</param>
        /// <param name="subSecondDecimals">Timestamp sub-second precision</param>
        /// <returns></returns>
        public static LoggerConfiguration EventCollector(
            this LoggerSinkConfiguration configuration,
            string splunkHost,
            string eventCollectorToken,
            CustomFields fields,
            string uriPath = ConfigurationDefaults.DefaultEventCollectorPath,
            string source = ConfigurationDefaults.DefaultSource,
            string sourceType = ConfigurationDefaults.DefaultSourceType,
            string host = ConfigurationDefaults.DefaultHost,
            string index = ConfigurationDefaults.DefaultIndex,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            bool renderMessage = true,
            int batchIntervalInSeconds = 2,
            int batchSizeLimit = 100,
            int? queueLimit = null,
            HttpMessageHandler messageHandler = null,
            LoggingLevelSwitch levelSwitch = null,
            int subSecondDecimals = 3)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchSizeLimit,
                Period = TimeSpan.FromSeconds(batchIntervalInSeconds),
                EagerlyEmitFirstEvent = true,
                QueueLimit = queueLimit
            };

            var eventCollectorSink = new EventCollectorSink(
               splunkHost,
               eventCollectorToken,
               uriPath,
               source,
               sourceType,
               host,
               index,
               fields,
               formatProvider,
               renderTemplate,
               renderMessage,
               messageHandler,
               subSecondDecimals: subSecondDecimals
               );


            var batchingSink = new PeriodicBatchingSink(eventCollectorSink, batchingOptions);

            return configuration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
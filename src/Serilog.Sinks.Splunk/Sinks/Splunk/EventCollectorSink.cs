// Copyright 2016 Serilog Contributors
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

using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class EventCollectorSink : IBatchedLogEventSink
    {
        internal const int DefaultQueueLimit = 100000;

        private readonly string _splunkHost;
        private readonly string _uriPath;
        private readonly ITextFormatter _jsonFormatter;
        private readonly EventCollectorClient _httpClient;


        /// <summary>
        /// Taken from Splunk.Logging.Common
        /// </summary>
        private static readonly HttpStatusCode[] HttpEventCollectorApplicationErrors =
        {
            HttpStatusCode.Forbidden,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.BadRequest
        };

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="formatProvider">The format provider used when rendering the message</param>
        /// <param name="renderTemplate">Whether to render the message template</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter from output JSON message.</param>
        /// <param name="subSecondPrecision">Timestamp sub-second precision</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            bool renderMessage = true,
            SubSecondPrecision subSecondPrecision = SubSecondPrecision.Milliseconds)
            : this(
                splunkHost,
                eventCollectorToken,
                null, null, null, null, null,
                formatProvider,
                renderTemplate,
                renderMessage,
                subSecondPrecision: subSecondPrecision)
        {
        }

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="formatProvider">The format provider used when rendering the message</param>
        /// <param name="renderTemplate">Whether to render the message template</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter from output JSON message.</param>
        /// <param name="subSecondPrecision">Timestamp sub-second precision</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            string uriPath,
            string source,
            string sourceType,
            string host,
            string index,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            bool renderMessage = true,
            HttpMessageHandler messageHandler = null,
            SubSecondPrecision subSecondPrecision = SubSecondPrecision.Milliseconds)
            : this(
                splunkHost,
                eventCollectorToken,
                uriPath,
                new SplunkJsonFormatter(renderTemplate, renderMessage, formatProvider, source, sourceType, host, index, subSecondPrecision: subSecondPrecision),
                messageHandler)
        {
        }

        /// <summary>
        /// Creates a new instance of the sink with Customfields
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="formatProvider">The format provider used when rendering the message</param>
        /// <param name="renderTemplate">Whether to render the message template</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="fields">Add extra CustomExtraFields for Splunk to index</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter from output JSON message.</param>
        /// <param name="subSecondPrecision">Timestamp sub-second precision</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            string uriPath,
            string source,
            string sourceType,
            string host,
            string index,
            CustomFields fields,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            bool renderMessage = true,
            HttpMessageHandler messageHandler = null,
            SubSecondPrecision subSecondPrecision = SubSecondPrecision.Milliseconds)
            // TODO here is the jsonformatter creation. We must make way to test output of jsonformatter. 
            : this(
                splunkHost,
                eventCollectorToken,
                uriPath,
                new SplunkJsonFormatter(renderTemplate, renderMessage, formatProvider, source, sourceType, host, index, fields, subSecondPrecision: subSecondPrecision),
                messageHandler)
        {
        }

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="uriPath">Change the default endpoint of the Event Collector e.g. services/collector/event</param>
        /// <param name="jsonFormatter">The text formatter used to render log events into a JSON format for consumption by Splunk</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            string uriPath,
            ITextFormatter jsonFormatter,
            HttpMessageHandler messageHandler = null)
        {
            _uriPath = uriPath;
            _splunkHost = splunkHost;
            _jsonFormatter = jsonFormatter;

            _httpClient = messageHandler != null
                ? new EventCollectorClient(eventCollectorToken, messageHandler)
                : new EventCollectorClient(eventCollectorToken);
        }

        /// <summary>
        ///     Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        public virtual async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var allEvents = new StringWriter();

            foreach (var logEvent in events)
            {
                _jsonFormatter.Format(logEvent, allEvents);
            }

            var request = new EventCollectorRequest(_splunkHost, allEvents.ToString(), _uriPath);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                //Application Errors sent via HTTP Event Collector
                if (HttpEventCollectorApplicationErrors.Any(x => x == response.StatusCode))
                {
                    // By not throwing an exception here the PeriodicBatchingSink will assume the batch succeeded and not send it again.
                    SelfLog.WriteLine(
                        "A status code of {0} was received when attempting to send to {1}.  The event has been discarded and will not be placed back in the queue.",
                        response.StatusCode.ToString(), _splunkHost);
                }
                else
                {
                    // EnsureSuccessStatusCode will throw an exception and the PeriodicBatchingSink will catch/log the exception and retry the batch.
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        /// <inheritdoc />
        public Task OnEmptyBatchAsync() => Task.CompletedTask;
    }
}

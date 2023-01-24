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

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class EventCollectorAuditSink : ILogEventSink
    {
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
        public EventCollectorAuditSink(
            string splunkHost,
            string eventCollectorToken,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true)
            : this(
                splunkHost,
                eventCollectorToken,
                null, null, null, null, null,
                formatProvider,
                renderTemplate)
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
        public EventCollectorAuditSink(
            string splunkHost,
            string eventCollectorToken,
            string uriPath,
            string source,
            string sourceType,
            string host,
            string index,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true,
            HttpMessageHandler messageHandler = null)
            : this(
                splunkHost,
                eventCollectorToken,
                uriPath,
                new SplunkJsonFormatter(renderTemplate, formatProvider, source, sourceType, host, index),
                messageHandler)
        {
        }

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="uriPath"></param>
        /// <param name="jsonFormatter">The text formatter used to render log events into a JSON format for consumption by Splunk</param>
        /// <param name="messageHandler">The handler used to send HTTP requests</param>
        public EventCollectorAuditSink(
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
        /// Emit a single log event which will be audited
        /// </summary>
        /// <param name="logEvent">The event to emit</param>
        public void Emit(LogEvent logEvent)
        {
            EmitAsync(logEvent).Wait();
        }

        private async Task EmitAsync(LogEvent logEvent)
        {
            var writer = new StringWriter();
            _jsonFormatter.Format(logEvent, writer);
            var request = new EventCollectorRequest(_splunkHost, writer.ToString(), _uriPath);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                //Application Errors sent via HTTP Event Collector
                if (HttpEventCollectorApplicationErrors.Any(x => x == response.StatusCode))
                {
                    SelfLog.WriteLine(
                        "A status code of {0} was received when attempting to send to {1}.",
                        response.StatusCode.ToString(), _splunkHost);
                    throw new Exception($"{response.StatusCode}");
                }

                response.EnsureSuccessStatusCode();
            }
        }
    }
}

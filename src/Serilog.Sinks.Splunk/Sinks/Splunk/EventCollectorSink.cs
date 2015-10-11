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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class EventCollectorSink : ILogEventSink
    {
        private readonly string _splunkHost;
        private readonly string _eventCollectorToken;
        private readonly string _source;
        private readonly string _sourceType;
        private readonly string _host;
        private readonly string _index;
        private readonly int _batchSizeLimitLimit;
        private readonly SplunkJsonFormatter _jsonFormatter;
        private readonly ConcurrentQueue<LogEvent> _queue;
        private readonly TimeSpan _batchInterval;
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
        /// <param name="batchSizeLimit">The size of the batch when sending to the event collector</param>
        /// <param name="formatProvider">The format provider used when rendering the message</param>
        /// <param name="renderTemplate">Whether to render the message template</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that batching should occur</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            int batchIntervalInSeconds = 5,
            int batchSizeLimit = 100,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true
            ) 
        {
            _splunkHost = splunkHost;
            _eventCollectorToken = eventCollectorToken;
            _queue = new ConcurrentQueue<LogEvent>();
            _jsonFormatter = new SplunkJsonFormatter(renderMessage: true, formatProvider: formatProvider, renderTemplate: renderTemplate);
            _batchSizeLimitLimit = batchSizeLimit;
            _batchInterval = TimeSpan.FromSeconds(batchIntervalInSeconds);

            _httpClient = new EventCollectorClient(_eventCollectorToken);

            //TODO: Implement handling similar to the Seq HTTP sink

            RepeatAction.OnInterval(_batchInterval, () => ProcessQueue().Wait(), new CancellationToken());

        }

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost">The host of the Splunk instance with the Event collector configured</param>
        /// <param name="eventCollectorToken">The token to use when authenticating with the event collector</param>
        /// <param name="batchSizeLimit">The size of the batch when sending to the event collector</param>
        /// <param name="formatProvider">The format provider used when rendering the message</param>
        /// <param name="renderTemplate">Whether to render the message template</param>
        /// <param name="batchIntervalInSeconds">The interval in seconds that batching should occur</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        public EventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            string source,
            string sourceType,
            string host,
            string index,
            int batchIntervalInSeconds = 5,
            int batchSizeLimit = 100,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true
            ) : this(splunkHost,
                eventCollectorToken,
                batchIntervalInSeconds,
                batchSizeLimit,
                formatProvider,
                renderTemplate)
        {
            _source = source;
            _sourceType = sourceType;
            _host = host;
            _index = index;
        }

        /// <summary>
        /// Emits the provided log event from a sink 
        /// </summary>
        /// <param name="logEvent"></param>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            _queue.Enqueue(logEvent);
        }

        private async Task ProcessQueue()
        {
            try
            {
                do
                {
                    var count = 0;
                    var events = new Queue<LogEvent>();
                    LogEvent next;

                    while (count < _batchSizeLimitLimit && _queue.TryDequeue(out next))
                    {
                        count++;
                        events.Enqueue(next);
                    }

                    if (events.Count == 0)
                        return;

                    //TODO: Add streaming capability for performance.  
                    // - Stream writer needs to move to outer scope.
                    // - Change Event Collector Request to only take string (or stream) of events and host
                    // - New object to stream many events as collection

                    foreach (var logEvent in events)
                    {
                        var sw = new StringWriter();

                        _jsonFormatter.Format(logEvent, sw);
                        var le = sw.ToString();
                        var request = new EventCollectorRequest(_splunkHost, le, _source, _sourceType, _host, _index);
                        var response = await _httpClient.SendAsync(request);

                            if (response.IsSuccessStatusCode) {  //Do Nothing?
                        }
                        else
                        {
                            //Application Errors sent via HTTP Event Collector
                            if (HttpEventCollectorApplicationErrors.Any(x => x == response.StatusCode))
                            {
                                SelfLog.WriteLine("A status code of {0} was received when attempting to send to {1}.  The event has been discarded and will not be placed back in the queue.", response.StatusCode.ToString(), _splunkHost);
                            }
                            else
                            {
                                //Put the item back in the queue & retry on next go
                                SelfLog.WriteLine("A status code of {0} was received when attempting to send to {1}.  The event has been placed back in the queue", response.StatusCode.ToString(), _splunkHost);

                                _queue.Enqueue(logEvent);
                            }
                        }
                        
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Exception while emitting batch from {0}: {1}", this, ex);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
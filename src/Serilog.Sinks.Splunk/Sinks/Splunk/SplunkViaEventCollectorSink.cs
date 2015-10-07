using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Sinks.Splunk
{
    internal class EventCollectorClient : HttpClient
    {
        public EventCollectorClient(string eventCollectorToken) : base()
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", eventCollectorToken);
        }
    }

    internal class EventCollectorRequest : HttpRequestMessage
    {
        public EventCollectorRequest(string splunkHost, string logEvent)
        {
            var jsonPayLoad = @"{""event"":" + logEvent + "}"
                .Replace("\r\n", string.Empty);

            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(splunkHost);
            Content = stringContent;
            Method = HttpMethod.Post;
        }
    }



    /// <summary>
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class SplunkViaEventCollectorSink : ILogEventSink
    {
        private readonly string _splunkHost;
        private readonly string _eventCollectorToken;
        private readonly int _batchSizeLimitLimit;
        private readonly SplunkJsonFormatter _jsonFormatter;
        private readonly ConcurrentQueue<LogEvent> _queue;
        private readonly TimeSpan _batchInterval;

        private static readonly HttpStatusCode[] HttpEventCollectorApplicationErrors =
       {
            HttpStatusCode.Forbidden,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.BadRequest
        };

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost"></param>
        /// <param name="eventCollectorToken"></param>
        /// <param name="batchSizeLimit"></param>
        /// <param name="formatProvider"></param>
        /// <param name="renderTemplate"></param>
        /// <param name="batchIntervalInSeconds"></param>
        public SplunkViaEventCollectorSink(
            string splunkHost,
            string eventCollectorToken,
            int batchIntervalInSeconds = 10,
            int batchSizeLimit = 10,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true
            )
        {
            _splunkHost = splunkHost;
            _eventCollectorToken = eventCollectorToken;
            _queue = new ConcurrentQueue<LogEvent>();
            _jsonFormatter = new SplunkJsonFormatter(renderMessage: true, formatProvider: formatProvider, renderTemplate:renderTemplate);
            _batchSizeLimitLimit = batchSizeLimit;
            _batchInterval = TimeSpan.FromSeconds(batchIntervalInSeconds);

            RepeatAction.OnInterval(_batchInterval, () => ProcessQueue().Wait(), new CancellationToken());
        }

        /// <summary>
        /// 
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

                    //TODO: Add streaming capability for performance

                    using (var client = new EventCollectorClient(_eventCollectorToken))
                    {
                        foreach (var logEvent in events)
                        {
                            var sw = new StringWriter();

                            _jsonFormatter.Format(logEvent, sw);
                            var le = sw.ToString();
                            var request = new EventCollectorRequest(_splunkHost, le);
                            var response = await client.SendAsync(request);

                            if (response.IsSuccessStatusCode) {
                                //Do Nothing?
                            }
                            else
                            {
                                //Application Errors sent via HTTP Event Collector
                                if (HttpEventCollectorApplicationErrors.Any(x => x == response.StatusCode))
                                {
                                    SelfLog.WriteLine("A status code of {0} was received when attempting to send to {1}.  The event has been discarded", response.StatusCode.ToString(), _splunkHost);
                                }
                                else
                                {
                                    //Put the item back in the queue & retry on next go
                                    SelfLog.WriteLine("A status code of {0} was received when attempting to send to {1}.  The event has been placed back in the queue", response.StatusCode.ToString(), _splunkHost);

                                    _queue.Enqueue(logEvent);
                                }
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
    }
}
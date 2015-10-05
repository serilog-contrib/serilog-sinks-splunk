using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Helper methods for the HTTP Event Collector
    /// </summary>
    public class EventCollectorExtensions
    {
        /// <summary>
        /// A list of HTTP Codes that Splunk deem as an application error.
        /// </summary>
        public static readonly HttpStatusCode[] HttpEventCollectorApplicationErrors =
     {
            HttpStatusCode.Forbidden,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.BadRequest
        };
    }


    /// <summary>
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class SplunkViaEventCollectorSink : ILogEventSink
    {
        private  string _splunkHost;
        private  string _eventCollectorToken;
        private  int _batchSizeLimit;
        private  JsonFormatter _jsonFormatter;
        private  ConcurrentQueue<LogEvent> _queue;

        /// <summary>
        /// Creates a new instance of the sink
        /// </summary>
        /// <param name="splunkHost"></param>
        /// <param name="eventCollectorToken"></param>
        /// <param name="formatProvider"></param>
        /// <param name="renderTemplate"></param>
        public SplunkViaEventCollectorSink(string splunkHost,
            string eventCollectorToken,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true
            )
        {
            _splunkHost = splunkHost;
            _eventCollectorToken = eventCollectorToken;
            _queue = new ConcurrentQueue<LogEvent>();

            _jsonFormatter = new JsonFormatter(renderMessage: true, formatProvider: formatProvider);
            _batchSizeLimit = 1;
            var batchInterval = TimeSpan.FromSeconds(5);

            RepeatAction.OnInterval(batchInterval, () => ProcessQueue().Wait(), new CancellationToken());
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

                    while (count < _batchSizeLimit && _queue.TryDequeue(out next))
                    {
                        count++;
                        events.Enqueue(next);
                    }

                    if (events.Count == 0)
                        return;

                    var sw = new StringWriter();

                    foreach (var logEvent in events)
                    {
                        
                        _jsonFormatter.Format(logEvent, sw);

                        var logEventAsAString = sw.ToString();

                        
                        var plainPayload2 = @"{""event"":" + logEventAsAString + "}";
                        

                        using (var client = new HttpClient())
                        {


                            var stringContent = new StringContent(plainPayload2, Encoding.UTF8, "application/json");
                           
                            plainPayload2 = plainPayload2.Replace("\r\n", string.Empty);

 

                            var request = new HttpRequestMessage
                            {
                                RequestUri = new Uri(_splunkHost),
                                Content = stringContent
                            };

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", _eventCollectorToken);

                            request.Method = HttpMethod.Post;

                            var response = await client.SendAsync(request);

                            if (response.IsSuccessStatusCode)
                            {
                            }
                            else
                            {
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

    internal static class RepeatAction
    {
        public static Task OnInterval(TimeSpan pollInterval, Action action, CancellationToken token,
            TaskCreationOptions taskCreationOptions, TaskScheduler taskScheduler)
        {
            return Task.Factory.StartNew(() =>
            {
                for (;;)
                {
                    if (token.WaitCancellationRequested(pollInterval))
                        break;
                    action();
                }
            }, token, taskCreationOptions, taskScheduler);
        }

        public static Task OnInterval(TimeSpan pollInterval, Action action, CancellationToken token)
        {
            return OnInterval(pollInterval, action, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static bool WaitCancellationRequested(this CancellationToken token, TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
    /// A sink to log to the Event Collector available in Splunk 6.3
    /// </summary>
    public class SplunkViaEventCollectorSink : ILogEventSink
    {
        private  string _splunkHost;
        private  string _eventCollectorToken;
        private  int _batchSizeLimitLimit;
        private  JsonFormatter _jsonFormatter;
        private  ConcurrentQueue<LogEvent> _queue;
        private TimeSpan _batchInterval;


        //public const int DefaultBatchInterval = 10 * 1000; // 10 seconds
        //public const int DefaultBatchSize = 10 * 1024; // 10KB
        //public const int DefaultBatchCount = 10;


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

                    var sw = new StringWriter();
                    
                    //TODO: Check status code against defaults
                    //TODO: Put items back in queue if matching use case
                    //TODO: Change to use retry methods

      
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
}
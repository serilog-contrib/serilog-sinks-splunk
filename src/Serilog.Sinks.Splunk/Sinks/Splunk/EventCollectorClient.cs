using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Serilog.Sinks.Splunk
{
    internal class EventCollectorClient : HttpClient, IDisposable
    {
        public EventCollectorClient(string eventCollectorToken) : base()
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", eventCollectorToken);
        }

        public EventCollectorClient(string eventCollectorToken, HttpMessageHandler messageHandler) : base(messageHandler)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", eventCollectorToken);
        }
    }
}
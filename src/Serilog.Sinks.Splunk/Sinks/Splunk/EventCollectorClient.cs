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
    }
}
using System;
using System.Net.Http;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    internal class SplunkEvent
    {
        private string _payload;

        internal SplunkEvent(string logEvent, string source, string sourceType, string host, string index)
        {
            _payload = string.Empty;

            var jsonPayLoad = @"{""event"":" + logEvent
            .Replace("\r\n", string.Empty);

            if (!string.IsNullOrWhiteSpace(source))
            {
                jsonPayLoad = jsonPayLoad + @",""source"":""" + source + @"""";
            }
            if (!string.IsNullOrWhiteSpace(sourceType))
            {
                jsonPayLoad = jsonPayLoad + @",""sourceType"":""" + sourceType + @"""";
            }
            if (!string.IsNullOrWhiteSpace(host))
            {
                jsonPayLoad = jsonPayLoad + @",""host"":""" + host + @"""";
            }
            if (!string.IsNullOrWhiteSpace(index))
            {
                jsonPayLoad = jsonPayLoad + @",""index"":""" + index + @"""";
            }

            jsonPayLoad = jsonPayLoad + "}";
            _payload = jsonPayLoad;
        }

        public string Payload
        {
            get { return _payload; }
        }
    }

    internal class EventCollectorRequest : HttpRequestMessage
    {
        internal EventCollectorRequest(string splunkHost, string jsonPayLoad)
        {
            var hostUrl = $@"{splunkHost}/services/collector/event";

            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(hostUrl);
            Content = stringContent;
            Method = HttpMethod.Post;
        }
    }
}
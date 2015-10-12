using System;
using System.Net.Http;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    internal class EventCollectorRequest : HttpRequestMessage
    {
        internal EventCollectorRequest(string splunkHost, string jsonPayLoad) 
        {

            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(splunkHost);
            Content = stringContent;
            Method = HttpMethod.Post;
        }

        internal EventCollectorRequest(
            string splunkHost, 
            string logEvent, 
            string source,
            string sourceType, 
            string host, 
            string index)
        {

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

            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(splunkHost);
            Content = stringContent;
            Method = HttpMethod.Post;
        }
    }
}
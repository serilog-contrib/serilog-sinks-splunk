using System.Net;

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
}
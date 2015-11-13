using System.Net;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Defines connection info used to connect against Splunk
    /// using UDP.
    /// </summary>
    public class SplunkUdpSinkConnectionInfo
    {
        /// <summary>
        /// Splunk host.
        /// </summary>
        public IPAddress Host { get; }

        /// <summary>
        /// Splunk port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Creates an instance of <see cref="SplunkUdpSinkConnectionInfo"/> used
        /// for defining connection info for connecting using UDP against Splunk.
        /// </summary>
        /// <param name="host">Splunk host.</param>
        /// <param name="port">Splunk UDP port.</param>
        public SplunkUdpSinkConnectionInfo(string host, int port) : this(IPAddress.Parse(host), port){ }

        /// <summary>
        /// Creates an instance of <see cref="SplunkUdpSinkConnectionInfo"/> used
        /// for defining connection info for connecting using UDP against Splunk.
        /// </summary>
        /// <param name="host">Splunk host.</param>
        /// <param name="port">Splunk UDP port.</param>
        public SplunkUdpSinkConnectionInfo(IPAddress host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
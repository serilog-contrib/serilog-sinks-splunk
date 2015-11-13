using System.Net;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Defines connection info used to connect against Splunk
    /// using TCP.
    /// </summary>
    public class SplunkTcpSinkConnectionInfo
    {
        /// <summary>
        /// Default size of the socket writer queue.
        /// </summary>
        public const int DefaultMaxQueueSize = 10000;

        /// <summary>
        /// Splunk host.
        /// </summary>
        public IPAddress Host { get; }

        /// <summary>
        /// Splunk port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Max Queue size for the TCP socket writer.
        /// See <see cref="DefaultMaxQueueSize"/> for default value (10000).
        /// </summary>
        public int MaxQueueSize { get; set; } = DefaultMaxQueueSize;

        /// <summary>
        /// Creates an instance of <see cref="SplunkTcpSinkConnectionInfo"/> used
        /// for defining connection info for connecting using TCP against Splunk.
        /// </summary>
        /// <param name="host">Splunk host.</param>
        /// <param name="port">Splunk TCP port.</param>
        public SplunkTcpSinkConnectionInfo(string host, int port) : this(IPAddress.Parse(host), port){ }

        /// <summary>
        /// Creates an instance of <see cref="SplunkTcpSinkConnectionInfo"/> used
        /// for defining connection info for connecting using TCP against Splunk.
        /// </summary>
        /// <param name="host">Splunk host.</param>
        /// <param name="port">Splunk TCP port.</param>
        public SplunkTcpSinkConnectionInfo(IPAddress host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
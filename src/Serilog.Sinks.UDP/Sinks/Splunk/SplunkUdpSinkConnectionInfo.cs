// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        public SplunkUdpSinkConnectionInfo(string host, int port) : this(IPAddress.Parse(host), port) { }

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


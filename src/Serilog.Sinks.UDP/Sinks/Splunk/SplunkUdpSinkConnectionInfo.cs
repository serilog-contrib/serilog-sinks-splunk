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

using System;
using System.Net;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Defines connection info used to connect against Splunk
    /// using UDP.
    /// </summary>
    public class SplunkUdpSinkConnectionInfo
    {
        private int _queueSizeLimit;

        /// <summary>
        /// Splunk host.
        /// </summary>
        public IPAddress Host { get; }

        /// <summary>
        /// Splunk port.
        /// </summary>
        public int Port { get; }

        ///<summary>
        /// The maximum number of events to post in a single batch. Defaults to: 100.
        /// </summary>
        public int BatchPostingLimit { get; set; } = 100;


        ///<summary>
        /// The time to wait between checking for event batches. Defaults to 10 seconds.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);


        /// <summary>
        /// The maximum number of events that will be held in-memory while waiting to ship them to
        /// Splunk. Beyond this limit, events will be dropped. The default is 100,000. Has no effect on
        /// durable log shipping.
        /// </summary>
        public int QueueSizeLimit
        {
            get { return _queueSizeLimit; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(QueueSizeLimit), "Queue size limit must be non-zero.");
                _queueSizeLimit = value;
            }
        }
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


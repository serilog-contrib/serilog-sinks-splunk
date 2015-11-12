// Copyright 2014 Serilog Contributors
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
using System.IO;
using System.Net;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Splunk.Logging;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A sink that logs to Splunk over TCP
    /// </summary>
    public class TcpSink : ILogEventSink, IDisposable
    {
        readonly ITextFormatter _formatter;
        private TcpSocketWriter _writer;

        /// <summary>
        /// Creates an instance of the Splunk TCP Sink
        /// </summary>
        /// <param name="host">The Splunk Host</param>
        /// <param name="port">The UDP port configured in Splunk</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="renderTemplate">If true, the message template will be rendered</param>
        public TcpSink(
            string host,
            int port,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true) : this(IPAddress.Parse(host), port, formatProvider, renderTemplate)
        {
        }

        /// <summary>
        /// Creates an instance of the Splunk TCP Sink
        /// </summary>
        /// <param name="hostAddress">The Splunk Host</param>
        /// <param name="port">The UDP port configured in Splunk</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="renderTemplate">If true, the message template will be rendered</param>
        public TcpSink(
            IPAddress hostAddress,
            int port,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true)
        {
            _writer = CreateSocketWriter(hostAddress, port);
            _formatter = new SplunkJsonFormatter(renderMessage: true, formatProvider: formatProvider, renderTemplate: renderTemplate);
        }

        /// <summary>
        /// Creates an instance of the Splunk TCP sink.
        /// </summary>
        /// <param name="host">The Splunk Host</param>
        /// <param name="port">The UDP port configured in Splunk</param>
        /// <param name="formatter">Custom formatter to use if you e.g. do not want to use the JsonFormatter.</param>
        public TcpSink(
            string host,
            int port,
            ITextFormatter formatter)
        {
            _writer = CreateSocketWriter(IPAddress.Parse(host), port);
            _formatter = formatter;
        }

        private static TcpSocketWriter CreateSocketWriter(IPAddress hostAddress, int port)
        {
            var reconnectionPolicy = new ExponentialBackoffTcpReconnectionPolicy();

            return new TcpSocketWriter(hostAddress, port, reconnectionPolicy, 10000);
        }

        /// <inheritdoc/>
        public void Emit(LogEvent logEvent)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter())
                _formatter.Format(logEvent, sw);

            _writer.Enqueue(sb.ToString());
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _writer?.Dispose();
            _writer = null;
        }
    }
}


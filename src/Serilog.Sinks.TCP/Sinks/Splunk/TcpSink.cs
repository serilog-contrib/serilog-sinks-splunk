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

using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Splunk.Logging;
using System;
using System.IO;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A sink that logs to Splunk over TCP
    /// </summary>
    public class TcpSink : ILogEventSink, IDisposable
    {
        private readonly ITextFormatter _formatter;
        private readonly SplunkTcpSinkConnectionInfo _connectionInfo;
        private bool disposedValue = false;

        private TcpSocketWriter _writer;

        /// <summary>
        /// Creates an instance of the Splunk TCP Sink.
        /// </summary>
        /// <param name="connectionInfo">Connection info used for connecting against Splunk.</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="renderTemplate">If true, the message template will be rendered</param>
        /// <param name="renderMessage">Include "RenderedMessage" parameter in output JSON message.</param>
        public TcpSink(SplunkTcpSinkConnectionInfo connectionInfo, IFormatProvider formatProvider = null, bool renderTemplate = true, bool renderMessage = true)
            : this(connectionInfo, CreateDefaultFormatter(formatProvider, renderTemplate, renderMessage))
        {
        }

        /// <summary>
        /// Creates an instance of the Splunk TCP Sink.
        /// </summary>
        /// <param name="connectionInfo">Connection info used for connecting against Splunk.</param>
        /// <param name="formatter">Custom formatter to use if you e.g. do not want to use the JsonFormatter.</param>
        public TcpSink(SplunkTcpSinkConnectionInfo connectionInfo, ITextFormatter formatter)
        {
            _connectionInfo = connectionInfo;
            _formatter = formatter;
            _writer = CreateSocketWriter(connectionInfo);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _writer?.Dispose();
                    _writer = null;
                }
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc/>
        public void Emit(LogEvent logEvent)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
                _formatter.Format(logEvent, sw);

            _writer.Enqueue(sb.ToString());
        }

        private static TcpSocketWriter CreateSocketWriter(SplunkTcpSinkConnectionInfo connectionInfo)
        {
            var reconnectionPolicy = new ExponentialBackoffTcpReconnectionPolicy();

            return new TcpSocketWriter(connectionInfo.Host, connectionInfo.Port, reconnectionPolicy, connectionInfo.MaxQueueSize);
        }

        private static SplunkJsonFormatter CreateDefaultFormatter(IFormatProvider formatProvider, bool renderTemplate, bool renderMessage = true)
        {
            return new SplunkJsonFormatter(renderTemplate, renderMessage, formatProvider);
        }
    }
}

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
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Renders log events into a default JSON format for consumption by Splunk.
    /// </summary>
    public class SplunkJsonFormatter : ITextFormatter
    {
        static readonly JsonValueFormatter ValueFormatter = new JsonValueFormatter();

        readonly bool _renderTemplate;
        readonly bool _renderMessage;
        readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Construct a <see cref="SplunkJsonFormatter"/>.
        /// </summary>
        /// <param name="renderMessage">If true, the message will be rendered and written to the output as a
        /// property named RenderedMessage.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the template used will be rendered and written to the output as a property named MessageTemplate</param>
        public SplunkJsonFormatter(
            bool renderTemplate = true,
            bool renderMessage = false,
            IFormatProvider formatProvider = null)
        {
            _renderTemplate = renderTemplate;
            _renderMessage = renderMessage;
            _formatProvider = formatProvider;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.Write("{\"Timestamp\":\"");
            output.Write(logEvent.Timestamp.ToString("o"));
            output.Write("\",\"Level\":\"");
            output.Write(logEvent.Level);

            if (_renderTemplate)
            {
                output.Write("\",\"MessageTemplate\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);
            }

            if (_renderMessage)
            {
                output.Write("\",\"RenderedMessage\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.RenderMessage(_formatProvider), output);
            }

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
            }

            if (logEvent.Properties.Count != 0)
                WriteProperties(logEvent.Properties, output);

            output.Write('}');
            output.WriteLine();
        }

        static void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            output.Write(",\"Properties\":{");

            var precedingDelimiter = "";
            foreach (var property in properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                ValueFormatter.Format(property.Value, output);
            }

            output.Write('}');
        }
    }
}

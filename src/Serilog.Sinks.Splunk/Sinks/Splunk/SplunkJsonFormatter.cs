// Copyright 2014 Serilog Contriutors
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
// limitations under the License.using System;

using System;
using System.IO;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A json formatter to allow conditional rendering of properties
    /// </summary>
    public class SplunkJsonFormatter : JsonFormatter
    {
        private readonly bool _renderTemplate;

        /// <summary>
        /// Construct a <see cref="JsonFormatter"/>.
        /// </summary>
        /// <param name="omitEnclosingObject">If true, the properties of the event will be written to
        /// the output without enclosing braces. Otherwise, if false, each event will be written as a well-formed
        /// JSON object.</param>
        /// <param name="closingDelimiter">A string that will be written after each log event is formatted.
        /// If null, <see cref="Environment.NewLine"/> will be used. Ignored if <paramref name="omitEnclosingObject"/>
        /// is true.</param>
        /// <param name="renderMessage">If true, the message will be rendered and written to the output as a
        /// property named RenderedMessage.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the template used will be rendered and written to the output as a property named MessageTemplate</param>
        public SplunkJsonFormatter(
            bool omitEnclosingObject = false,
            string closingDelimiter = null,
            bool renderMessage = false,
            IFormatProvider formatProvider = null,
            bool renderTemplate = true) 
            :base(omitEnclosingObject,closingDelimiter,renderMessage,formatProvider)
        {
            _renderTemplate = renderTemplate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="delim"></param>
        /// <param name="output"></param>
        protected override void WriteMessageTemplate(string template, ref string delim, TextWriter output)
        {
            if(_renderTemplate)
                base.WriteMessageTemplate(template, ref delim, output);
        }
    }
}
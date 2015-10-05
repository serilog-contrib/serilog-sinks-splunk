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
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Splunk;
using Splunk.Client;

namespace Serilog
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerConfigurationSplunkExtensions
    {

        internal const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="splunkHost"></param>
        /// <param name="eventCollectorToken"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="renderTemplate"></param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaEventCollector(
            this LoggerSinkConfiguration sinkConfiguration,
            string splunkHost,
            string eventCollectorToken,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null, bool renderTemplate = true)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new SplunkViaEventCollectorSink(splunkHost, eventCollectorToken), restrictedToMinimumLevel);
        }
    }
}
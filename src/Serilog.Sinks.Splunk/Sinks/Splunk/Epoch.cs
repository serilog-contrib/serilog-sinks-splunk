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

namespace Serilog.Sinks.Splunk
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides extension methods for DateTimeOffset to convert to epoch time.
    /// </summary>
    public static class EpochExtensions
    {
        private static DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        const string MillisecondFormat = "#.000";
        const string MicrosecondFormat = "#.000000";
        const string NanosecondFormat = "#.000000000";

        /// <summary>
        /// Converts a DateTimeOffset value to epoch time with specified sub-second precision.
        /// </summary>
        /// <param name="value">The DateTimeOffset value to convert.</param>
        /// <param name="subSecondPrecision">Timestamp sub-second precision.</param>
        /// <returns>The epoch time representation of the DateTimeOffset value.</returns>
        public static string ToEpoch(this DateTimeOffset value, SubSecondPrecision subSecondPrecision = SubSecondPrecision.Milliseconds)
        {
            // From Splunk HTTP Collector Protocol
            // The default time format is epoch time format, in the format <sec>.<ms>. 
            // For example, 1433188255.500 indicates 1433188255 seconds and 500 milliseconds after epoch, 
            // or Monday, June 1, 2015, at 7:50:55 PM GMT.
            // See: https://docs.splunk.com/Documentation/Splunk/9.2.0/SearchReference/Commontimeformatvariables

            var totalSeconds = ToSeconds(value.ToUniversalTime().Ticks - Epoch.Ticks);
            var format = subSecondPrecision switch {
                SubSecondPrecision.Nanoseconds => NanosecondFormat,
                SubSecondPrecision.Microseconds => MicrosecondFormat,
                _ => MillisecondFormat,
            };

            return Math.Round(totalSeconds, (int)subSecondPrecision, MidpointRounding.AwayFromZero).ToString(format, CultureInfo.InvariantCulture);
        }

        private static decimal ToSeconds(long ticks)
        {
            long wholeSecondPortion = (ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond;
            long subsecondPortion = ticks - wholeSecondPortion;
            decimal wholeSeconds = wholeSecondPortion / (decimal)TimeSpan.TicksPerSecond;
            decimal subseconds = subsecondPortion / (decimal)TimeSpan.TicksPerSecond;
            return wholeSeconds + subseconds;
        }
    }
}

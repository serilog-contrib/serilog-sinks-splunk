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

    internal static class EpochExtensions
    {
        private static DateTimeOffset Epoch = new DateTimeOffset(1970,1,1,0,0,0,TimeSpan.Zero);

        public static double ToEpoch(this DateTimeOffset value)
        {
            // From Splunk HTTP Collector Protocol
            // The default time format is epoch time format, in the format <sec>.<ms>. 
            // For example, 1433188255.500 indicates 1433188255 seconds and 500 milliseconds after epoch, 
            // or Monday, June 1, 2015, at 7:50:55 PM GMT.
            // See: http://dev.splunk.com/view/SP-CAAAE6P

            return Math.Round((value - Epoch).TotalSeconds, 3, MidpointRounding.AwayFromZero);
        }
    }
}
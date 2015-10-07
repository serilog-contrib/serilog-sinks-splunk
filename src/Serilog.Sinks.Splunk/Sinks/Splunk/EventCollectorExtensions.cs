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

using System.Net;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Helper methods for the HTTP Event Collector
    /// </summary>
    public class EventCollectorExtensions
    {
        /// <summary>
        /// A list of HTTP Codes that Splunk deem as an application error.
        /// </summary>
        public static readonly HttpStatusCode[] HttpEventCollectorApplicationErrors =
     {
            HttpStatusCode.Forbidden,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.BadRequest
        };
    }
}
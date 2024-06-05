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
using System.Net.Http;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    internal class EventCollectorRequest : HttpRequestMessage
    {
        internal EventCollectorRequest(string splunkHost, string jsonPayLoad, string uri = ConfigurationDefaults.DefaultEventCollectorPath)
        {
            var hostUrl = splunkHost.Contains(ConfigurationDefaults.DefaultCollectorPath)
                ? splunkHost
                : $"{splunkHost.TrimEnd('/')}/{uri.TrimStart('/').TrimEnd('/')}";
           
            RequestUri = new Uri(hostUrl);
            Content = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            Method = HttpMethod.Post;
        }
    }
}

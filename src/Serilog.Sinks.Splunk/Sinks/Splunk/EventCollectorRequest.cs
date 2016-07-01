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
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    internal class SplunkEvent
    {
        private string _payload;

        internal SplunkEvent(string logEvent, string source, string sourceType, string host, string index, double time)
        {
            _payload = string.Empty;

            var jsonPayLoad = @"{""event"":" + logEvent
            .Replace("\r\n", string.Empty);

            if (!string.IsNullOrWhiteSpace(source))
            {
                jsonPayLoad = jsonPayLoad + @",""source"":""" + source + @"""";
            }
            if (!string.IsNullOrWhiteSpace(sourceType))
            {
                jsonPayLoad = jsonPayLoad + @",""sourceType"":""" + sourceType + @"""";
            }
            if (!string.IsNullOrWhiteSpace(host))
            {
                jsonPayLoad = jsonPayLoad + @",""host"":""" + host + @"""";
            }
            if (!string.IsNullOrWhiteSpace(index))
            {
                jsonPayLoad = jsonPayLoad + @",""index"":""" + index + @"""";
            }

            if (time > 0)
            {
                jsonPayLoad = jsonPayLoad + @",""time"":" +  time.ToString(CultureInfo.InvariantCulture);
            }

            jsonPayLoad = jsonPayLoad + "}";
            _payload = jsonPayLoad;
        }

        public string Payload
        {
            get { return _payload; }
        }
    }

    internal class EventCollectorRequest : HttpRequestMessage
    {
        internal EventCollectorRequest(string splunkHost, string jsonPayLoad, string uri ="services/collector")
        {
            var hostUrl = $@"{splunkHost}/{uri}";

            if(splunkHost.Contains("services/collector"))
            {
                hostUrl = $@"{splunkHost}";
            }
            
            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(hostUrl);
            Content = stringContent;
            Method = HttpMethod.Post;
        }
    }
}
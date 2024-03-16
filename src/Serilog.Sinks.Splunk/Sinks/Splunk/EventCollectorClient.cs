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
using System.Net.Http.Headers;

namespace Serilog.Sinks.Splunk
{
    internal class EventCollectorClient : HttpClient, IDisposable
    {
        private const string AUTH_SCHEME = "Splunk";
        private const string SPLUNK_REQUEST_CHANNEL = "X-Splunk-Request-Channel";

        public EventCollectorClient(string eventCollectorToken) : base()
        {
            SetHeaders(eventCollectorToken);
        }

        public EventCollectorClient(string eventCollectorToken, HttpMessageHandler messageHandler) : base(messageHandler)
        {
            SetHeaders(eventCollectorToken);
        }

        private void SetHeaders(string eventCollectorToken)
        {
            // Reminder: If the event collector url is redirected, all authentication headers will be removed.
            // See: https://github.com/dotnet/runtime/blob/ccfe21882e4a2206ce49cd5b32d3eb3cab3e530f/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/RedirectHandler.cs#L53

            DefaultRequestHeaders.Authorization ??= new AuthenticationHeaderValue(AUTH_SCHEME, eventCollectorToken);
            
            if (!this.DefaultRequestHeaders.Contains(SPLUNK_REQUEST_CHANNEL))
            {
                this.DefaultRequestHeaders.Add(SPLUNK_REQUEST_CHANNEL, Guid.NewGuid().ToString());
            }
        }
    }
}
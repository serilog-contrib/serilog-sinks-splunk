using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Splunk;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Splunk.Tests
{
    public class EventCollectorSinkTests
    {
        /// <summary>
        /// A fake HttpMessageHandler that records disposal and captures outgoing requests.
        /// </summary>
        private class FakeHandler : HttpMessageHandler
        {
            public bool Disposed { get; private set; }
            public HttpRequestMessage LastRequest { get; private set; }
            public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(ResponseStatusCode));
            }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        [Fact]
        public void DisposingSinkDisposesOwnedHttpResources()
        {
            var handler = new FakeHandler();
            var sink = new EventCollectorSink(
                "http://splunk.example.com:8088",
                "test-token",
                null,
                new SplunkJsonFormatter(false, true, null),
                handler);

            sink.Dispose();

            Assert.True(handler.Disposed, "HttpMessageHandler should be disposed when sink is disposed");
        }

        [Fact]
        public void DisposingSinkTwiceDoesNotThrow()
        {
            var handler = new FakeHandler();
            var sink = new EventCollectorSink(
                "http://splunk.example.com:8088",
                "test-token",
                null,
                new SplunkJsonFormatter(false, true, null),
                handler);

            sink.Dispose();
            sink.Dispose(); // Should not throw
        }

        [Fact]
        public async Task EmitBatchSendsRequestWithCorrectAuthHeaders()
        {
            var handler = new FakeHandler();
            var sink = new EventCollectorSink(
                "http://splunk.example.com:8088",
                "my-secret-token",
                "services/collector/event",
                new SplunkJsonFormatter(false, true, null),
                handler);

            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                null,
                new MessageTemplate("Test", new List<MessageTemplateToken>()),
                new LogEventProperty[] { });

            await sink.EmitBatchAsync(new[] { logEvent });

            Assert.NotNull(handler.LastRequest);
            Assert.Equal("Splunk", handler.LastRequest.Headers.Authorization?.Scheme);
            Assert.Equal("my-secret-token", handler.LastRequest.Headers.Authorization?.Parameter);
            Assert.True(handler.LastRequest.Headers.Contains("X-Splunk-Request-Channel"));

            sink.Dispose();
        }

        [Fact]
        public void DisposingLoggerDisposesHttpResources()
        {
            var handler = new FakeHandler();

            var logger = new LoggerConfiguration()
                .WriteTo.EventCollector(
                    "http://splunk.example.com:8088",
                    "test-token",
                    new SplunkJsonFormatter(false, true, null),
                    messageHandler: handler)
                .CreateLogger();

            logger.Information("Test event");

            // Logger disposal should cascade to sink disposal
            logger.Dispose();

            Assert.True(handler.Disposed, "HttpMessageHandler should be disposed when logger is disposed");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Xunit;
using Serilog.Sinks.Splunk.Tests.Support;

namespace Serilog.Sinks.Splunk.Tests
{
    public class SplunkJsonFormatterTests
    {
        void AssertValidJson(Action<ILogger> act, 
            string source = "",
            string sourceType= "",
            string host= "",
            string index= "",
            CustomFields fields=null)
        {
            StringWriter outputRendered = new StringWriter(), output = new StringWriter();
            var log = new LoggerConfiguration()
                .WriteTo.Sink(new TextWriterSink(output, new SplunkJsonFormatter(false, null, source, sourceType, host, index)))
                .WriteTo.Sink(new TextWriterSink(outputRendered, new SplunkJsonFormatter(true, null, source, sourceType, host, index)))
                .CreateLogger();

            act(log);

            // Unfortunately this will not detect all JSON formatting issues; better than nothing however.
            JObject.Parse(output.ToString());
            JObject.Parse(outputRendered.ToString());
        }

        [Fact]
        public void AnEmptyEventIsValidJson()
        {
            AssertValidJson(log => log.Information("No properties"));
        }

        [Fact]
        public void AMinimalEventIsValidJson()
        {
            AssertValidJson(log => log.Information("One {Property}", 42));
        }

        [Fact]
        public void MultiplePropertiesAreDelimited()
        {
            AssertValidJson(log => log.Information("Property {First} and {Second}", "One", "Two"));
        }

        [Fact]
        public void ExceptionsAreFormattedToValidJson()
        {
            AssertValidJson(log => log.Information(new DivideByZeroException(), "With exception"));
        }

        [Fact]
        public void ExceptionAndPropertiesAreValidJson()
        {
            AssertValidJson(log => log.Information(new DivideByZeroException(), "With exception and {Property}", 42));
        }

        [Fact]
        public void AMinimalEventWithSourceIsValidJson()
        {
            AssertValidJson(log => log.Information("One {Property}", 42), source: "A Test Source");
        }
        
        [Fact]
        public void AMinimalEventWithSourceTypeIsValidJson()
        {
            AssertValidJson(log => log.Information("One {Property}", 42), sourceType: "A Test SourceType");
        }

        [Fact]
        public void AMinimalEventWithHostIsValidJson()
        {
            AssertValidJson(log => log.Information("One {Property}", 42), host: "A Test Host");
        }

        [Fact]
        public void AMinimalEventWithIndexIsValidJson()
        {
            AssertValidJson(log => log.Information("One {Property}", 42), host: "testindex");
        }
        [Fact]
        public void EventWithCustomFields()
        {
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("relChan", "Test"),
                new CustomField("version", "17.8.9.beta"),
                new CustomField("rel", "REL1706"),
                new CustomField("role", new List<string>() { "service", "rest", "ESB" })
            });
            AssertValidJson(log => log.Information("One {Property}", 42), fields: metaData);
        }

    }
}
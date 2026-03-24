using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Splunk.Tests.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;
namespace Serilog.Sinks.Splunk.Tests
{

    public class SplunkJsonFormatterTests
    {
        void AssertValidJson(Action<ILogger> act,
            string source = "",
            string sourceType = "",
            string host = "",
            string index = "",
            CustomFields fields = null)
        {
            StringWriter outputRendered = new StringWriter(), output = new StringWriter();
            var log = new LoggerConfiguration()
                .WriteTo.Sink(new TextWriterSink(output, new SplunkJsonFormatter(false, true, null, source, sourceType, host, index)))
                .WriteTo.Sink(new TextWriterSink(outputRendered, new SplunkJsonFormatter(true, true, null, source, sourceType, host, index)))
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

        [Fact]
        public void Test_CustomFields_Jsonformatter_for_Splunk_Sink()
        {
            //Arrange
            int a = 1;
            int b = 0;
            // Here we set up some made up CustomFields that we want splunk to index for every event so we could filter on them
            // Eg could be like in this example releasechannel eg Dev,Test,AccepteanceTest, prod ; version of the code, Release, 
            // role is an example of when a field property has been set to a multi-value JSON array. See: http://dev.splunk.com/view/event-collector/SP-CAAAFB6
            // Could be used to describe a hierachy in dimension. Here it is a service of type rest for the Enterprise Service Bus
            // these field would of course be different for every Company. But should be conformed over the organisation. Just like Ralph Kimball conformed dimensions for BI. 
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("relChan", "Test"),
                new CustomField("version", "17.8.9.beta"),
                new CustomField("rel", "REL1706"),
                new CustomField("role", new List<string>() { "Backend", "Service", "Rest" })
            });

            TextWriter eventtTextWriter = new StringWriter();
            Exception testException = null;
            var timeStamp = DateTimeOffset.Now;
            // var timeStampUnix = (Math.Round(timeStamp.ToUnixTimeMilliseconds() / 1000.0, 3)).ToString("##.###", CultureInfo.InvariantCulture); //req dotnet 4.6.2
            var timeStampUnix = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString("##.###", CultureInfo.InvariantCulture);
            var sut = new SplunkJsonFormatter(renderTemplate: true, renderMessage: true, formatProvider: null, source: "BackPackTestServerChannel", sourceType: "_json", host: "Wanda", index: "Main", customFields: metaData);
            try
            {
                var willnotwork = a / b;
            }
            catch (Exception e)
            {
                testException = e;
            }
            var logEvent = new LogEvent(timestamp: timeStamp, level: LogEventLevel.Debug, exception: testException
                , messageTemplate: new MessageTemplate("Should be an div by zero error", new List<MessageTemplateToken>())
                , properties: new LogEventProperty[]
                    {
                      new LogEventProperty("Serilog.Sinks.Splunk.Sample", new ScalarValue("ViaEventCollector"))
                    , new LogEventProperty("Serilog.Sinks.Splunk.Sample.TestType", new ScalarValue("AddCustomFields"))
                    }
                );
            //Act
            sut.Format(logEvent, eventtTextWriter);
            var resultJson = eventtTextWriter.ToString();
            TestEventResultObject testEventResult = JsonConvert.DeserializeObject<TestEventResultObject>(resultJson);
            //Assert
            Assert.True(String.Equals(testEventResult.Host, "Wanda"), "Wanda should be my  host see the movie, else the JsonFormater is wack in test"); // "Wanda should be my  host see the movie, else the JsonFormater is wack in test");                                                                                                                                                       // I do no seem to get the div part right. Something strange with  round or how the json timestamp is calculated. I am practical and only check the whole part for now.
            var timestampPartFromTest = testEventResult.Time.Split('.')[0];
            var timeShouldBe = timeStampUnix.Split('.')[0];
            Assert.True(String.Equals(timestampPartFromTest, timeShouldBe), "Json Time stamp is off ");
            Assert.True(String.Equals(testEventResult.Source, "BackPackTestServerChannel"), "Jsonformater do not se the Splunk field source to the right value");
            Assert.True(String.Equals(testEventResult.Sourcetype, "_json"), "Jsonformater do not se the Splunk field sourcetype to the right value _json");
            Assert.True(testEventResult.@Event.Exception.StartsWith("System.DivideByZeroException:"), "Exception Does not seem to be right after JsonFormater no DivideByZeroText ");
            // StringAssert.IsMatch("AddCustomFields", testEventResult.@event.Exception, "Exception Does not seem to be right after JsonFormater no AddCustomField ");
            Assert.True(String.Equals(testEventResult.@Event.Level, LogEventLevel.Debug.ToString()), "Siri LogEvent should be Debug");
            //Now finally we start to check the CustomField
            Assert.True(String.Equals(testEventResult.Fields.RelChan, "Test"), "CustomField RelChan is not correct after format for Splunkjsonformatter");
            Assert.True(String.Equals(testEventResult.Fields.Version, "17.8.9.beta"), "CustomField Version is not correct after format for Splunkjsonformatter");
            Assert.True(String.Equals(testEventResult.Fields.Rel, "REL1706"), "CustomField rel is not correct after format for Splunkjsonformatter");
            Assert.True(String.Equals(testEventResult.Fields.Role[0], "Backend"), "CustomField Role array 0 is not correct after format for Splunkjsonformatter");
            Assert.True(String.Equals(testEventResult.Fields.Role[1], "Service"), "CustomField Role array 1 correct after format for Splunkjsonformatter");
            Assert.True(String.Equals(testEventResult.Fields.Role[2], "Rest"), "CustomField Role array 2 correct after format for Splunkjsonformatter");
        }
        [Fact]
        public void CustomFieldsWithSpecialCharactersProduceValidJson()
        {
            // Arrange - field names and values containing characters that would break
            // JSON if not properly escaped: quotes, backslashes, newlines
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("field\"with\"quotes", "value\"with\"quotes"),
                new CustomField("field\\with\\backslashes", "value\\with\\backslashes"),
                new CustomField("field\nwith\nnewlines", "value\nwith\nnewlines"),
                new CustomField("field\twith\ttabs", "value\twith\ttabs"),
                new CustomField("normalField", "normalValue"),
                new CustomField("arrayField", new List<string> { "val\"1", "val\\2", "val\n3" })
            });

            var formatter = new SplunkJsonFormatter(
                renderTemplate: true,
                renderMessage: true,
                formatProvider: null,
                source: "testSource",
                sourceType: "_json",
                host: "testHost",
                index: "testIndex",
                customFields: metaData);

            var logEvent = new LogEvent(
                timestamp: DateTimeOffset.UtcNow,
                level: LogEventLevel.Information,
                exception: null,
                messageTemplate: new MessageTemplate("Test message", new List<MessageTemplateToken>()),
                properties: new LogEventProperty[] { });

            var output = new StringWriter();

            // Act
            formatter.Format(logEvent, output);
            var resultJson = output.ToString();

            // Assert - must parse as valid JSON
            var parsed = JObject.Parse(resultJson);

            // Verify the fields section exists and values round-trip correctly
            Assert.NotNull(parsed["fields"]);
            Assert.Equal("value\"with\"quotes", parsed["fields"]["field\"with\"quotes"]?.ToString());
            Assert.Equal("value\\with\\backslashes", parsed["fields"]["field\\with\\backslashes"]?.ToString());
            Assert.Equal("value\nwith\nnewlines", parsed["fields"]["field\nwith\nnewlines"]?.ToString());
            Assert.Equal("normalValue", parsed["fields"]["normalField"]?.ToString());

            // Verify array field round-trips
            var arrayValues = parsed["fields"]["arrayField"];
            Assert.NotNull(arrayValues);
            Assert.Equal("val\"1", arrayValues[0]?.ToString());
            Assert.Equal("val\\2", arrayValues[1]?.ToString());
            Assert.Equal("val\n3", arrayValues[2]?.ToString());
        }

        [Fact]
        public void CustomFieldsWithEmptyAndUnicodeValuesProduceValidJson()
        {
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("emptyValue", ""),
                new CustomField("unicodeField", "\u00e9\u00e8\u00ea\u00eb"),
                new CustomField("emojiField", "\ud83d\ude00\ud83d\ude01"),
                new CustomField("htmlLike", "<script>alert('xss')</script>"),
                new CustomField("multiArray", new List<string> { "", "normal", "<b>bold</b>" })
            });

            var formatter = new SplunkJsonFormatter(
                renderTemplate: false,
                renderMessage: true,
                formatProvider: null,
                source: "test",
                sourceType: "_json",
                host: "host",
                index: "idx",
                customFields: metaData);

            var logEvent = new LogEvent(
                timestamp: DateTimeOffset.UtcNow,
                level: LogEventLevel.Debug,
                exception: null,
                messageTemplate: new MessageTemplate("Test", new List<MessageTemplateToken>()),
                properties: new LogEventProperty[] { });

            var output = new StringWriter();
            formatter.Format(logEvent, output);
            var resultJson = output.ToString();

            // Must parse without error
            var parsed = JObject.Parse(resultJson);
            Assert.NotNull(parsed["fields"]);
            Assert.Equal("", parsed["fields"]["emptyValue"]?.ToString());
            Assert.Equal("\u00e9\u00e8\u00ea\u00eb", parsed["fields"]["unicodeField"]?.ToString());
            Assert.Equal("<script>alert('xss')</script>", parsed["fields"]["htmlLike"]?.ToString());
        }

        #region Test_CustomFields_Jsonformatter_for_Splunk_Sink_Help_Classes
        // http://json2csharp.com/#
        // https://github.com/JamesNK/Newtonsoft.Json
        public class Event
        {
            public string Level { get; set; }
            public string MessageTemplate { get; set; }
            public string RenderedMessage { get; set; }
            public string Exception { get; set; }
        }
        public class TestCustomFields
        {
            public string RelChan { get; set; }
            public string Version { get; set; }
            public string Rel { get; set; }
            public List<string> Role { get; set; }
        }
        public class TestEventResultObject
        {
            public string Time { get; set; }
            public Event @Event { get; set; }
            public string Source { get; set; }
            public string Sourcetype { get; set; }
            public string Host { get; set; }
            public string Index { get; set; }
            public TestCustomFields Fields { get; set; }

        }
        #endregion
    }
}
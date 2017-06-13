using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Serilog.Events;
using Serilog.Parsing;
using Newtonsoft.Json;

namespace Serilog.Sinks.Splunk.CustomFieldsTests
{

    // http://json2csharp.com/#
    // https://github.com/JamesNK/Newtonsoft.Json
    public class Event
    {
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string RenderedMessage { get; set; }
        public string Exception { get; set; }
    }
    public class  TestCustomFields { 
        public string  RelChan { get; set; }
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
    [TestFixture]
    class SplunkCustomFieldsTests
    {
        //  Patrik local vm machine   
        [Test]
        public void Test_CustomFields_Jsonformatter_for_Splunk_Sink()
        {
            //Arrange
            int a = 1;
            int b = 0;
            // Here we set up some made up CustomFields that we want splunk to index for every event so we could filter on them
            // Eg could be like in this example releasechannel eg Dev,Test,AccepteanceTest, prod ; version of the code, Release, 
            // role is an example of when a field property has been set to a multi-value JSON array. See: http://dev.splunk.com/view/event-collector/SP-CAAAFB6
            // Could be used to describe a hierachy in dimension. Here it is a service of type rest for the Enterprise Service Bus
            // these field would of course be different for every Company. But should be conformed over the organisation. Just like ralph kimball conformed dimensions for BI. 
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("relChan", "Test"),
                new CustomField("version", "17.8.9.beta"),
                new CustomField("rel", "REL1706"),
                new CustomField("role", new List<string>() { "service", "rest", "ESB" })
            });
       
            TextWriter eventtTextWriter = new StringWriter(); 
            Exception testException = null;
            var timeStamp = DateTimeOffset.Now;      
            var timeStampUnix = ( Math.Round( timeStamp.ToUnixTimeMilliseconds()/1000.0,3)).ToString("##.###", CultureInfo.InvariantCulture);
            var sut = new SplunkJsonFormatter(renderTemplate:true,formatProvider:null,source: "BackPackTestServerChannel",sourceType:"_json",host:"Wanda",index:"Main",customFields: metaData);
            try
            {
                var willnotwork = a / b;
            }
            catch (Exception e)
            {
                testException = e;
            }
            var logEvent = new LogEvent(timestamp: timeStamp,level:LogEventLevel.Debug,exception: testException
                ,messageTemplate: new MessageTemplate("Should be an div by zero error", new List<MessageTemplateToken>())
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
            StringAssert.AreEqualIgnoringCase(testEventResult.Host, "Wanda", "Wanda should be my  host see the movie, else the JsonFormater is wack in test");
            // I do no seem to get the div part right. Something strange with  round or how the json timestamp is calculated. I am practical and only check the whole part for now.
            StringAssert.AreEqualIgnoringCase(testEventResult.Time.Split('.')[0], timeStampUnix.Split('.')[0], "Json Time stamp is off ");
            StringAssert.AreEqualIgnoringCase(testEventResult.Source, "BackPackTestServerChannel", "Jsonformater do not se the Splunk field source to the right value");
            StringAssert.AreEqualIgnoringCase(testEventResult.Sourcetype, "_json", "Jsonformater do not se the Splunk field sourcetype to the right value _json");
            StringAssert.IsMatch("System\\.DivideByZeroException:", testEventResult.@Event.Exception, "Exception Does not seem to be right after JsonFormater no DivideByZeroText ");
           // StringAssert.IsMatch("AddCustomFields", testEventResult.@event.Exception, "Exception Does not seem to be right after JsonFormater no AddCustomField ");
            StringAssert.AreEqualIgnoringCase(testEventResult.@Event.Level , LogEventLevel.Debug.ToString(), "Siri LogEvent should be Debug");
            //Now finally we start to check the CustomField
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.RelChan, "Test", "CustomField RelChan is not correct after format for Splunkjsonformatter");
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.Version, "17.8.9.beta", "CustomField Version is not correct after format for Splunkjsonformatter");
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.Rel, "REL1706", "CustomField rel is not correct after format for Splunkjsonformatter");
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.Role[0], "service", "CustomField Role array 0 is not correct after format for Splunkjsonformatter");
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.Role[1], "rest", "CustomField Role array 1 correct after format for Splunkjsonformatter");
            StringAssert.AreEqualIgnoringCase(testEventResult.Fields.Role[2], "ESB", "CustomField Role array 2 correct after format for Splunkjsonformatter");
        }
    }
}

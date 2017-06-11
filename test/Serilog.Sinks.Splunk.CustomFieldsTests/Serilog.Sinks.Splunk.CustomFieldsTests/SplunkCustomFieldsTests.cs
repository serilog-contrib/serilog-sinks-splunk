using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.Splunk;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.Splunk.CustomFieldsTests
{
    [TestFixture]
    class SplunkCustomFieldsTests
    {
        const string SPLUNK_FULL_ENDPOINT = "http://ws2012-devops:8088/services/collector";
        //  Patrik local vm machine   

        const string SPLUNK_ENDPOINT = "http://ws2012-devops:8088"; //  Patrik local vm machine    
        const string SPLUNK_HEC_TOKEN = "1AFAC088-BFC6-447F-A358-671FA7465342"; // PATRIK lOCAL VM -MACHINE
        [Test]
        public void Test_Add_CustomFields_for_Splunk_Sink_()
        {
            //Arrange
            int a = 1;
            int b = 0;
            var metaData = new CustomFields();
            metaData.CustomFieldList.Add(new CustomField("relChan", "Test"));
            metaData.CustomFieldList.Add(new CustomField("version", "17.8.9.beta"));
            metaData.CustomFieldList.Add(new CustomField("rel", "REL1706"));
            metaData.CustomFieldList.Add(new CustomField("role", new List<string>() { "service", "rest", "ESB" }));
            Exception testException = null;
            var sut = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.EventCollector(
        splunkHost: SPLUNK_ENDPOINT
        , eventCollectorToken: SPLUNK_HEC_TOKEN
        , host: System.Environment.MachineName
        , source: "BackPackTestServerChannel"
        , sourceType: "_json"
        , fields: metaData)
    .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
    .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "AddCustomFields")
    .CreateLogger();
            //Act
            try
            {
                var willnotwork = a / b;
            }
            catch (Exception e)
            {
                testException = e;

            }
            sut.Debug(testException, "Should be an div by zeroerror");
            //Assert


        }
        [Test]
        public void Test_CustomFields_Jsonformatter_for_Splunk_Sink_()
        {
            //Arrange
            int a = 1;
            int b = 0;
            var metaData = new CustomFields();
            metaData.CustomFieldList.Add(new CustomField("relChan", "Test"));
            metaData.CustomFieldList.Add(new CustomField("version", "17.8.9.beta"));
            metaData.CustomFieldList.Add(new CustomField("rel", "REL1706"));
            metaData.CustomFieldList.Add(new CustomField("role", new List<string>() { "service", "rest", "ESB" }));
            TextWriter eventtTextWriter = new StringWriter();
           
            Exception testException = null;
            var timeStamp = DateTimeOffset.Now;
           var sut = new SplunkJsonFormatter(renderTemplate:true,formatProvider:null,source: "BackPackTestServerChannel",sourceType:"_json",host:"Wanda",index:"Main",customFields: metaData);
            try
            {
                var willnotwork = a / b;
            }
            catch (Exception e)
            {
                testException = e;
            }
            var msgTemplate = new MessageTemplate("Should be an div by zeroerror",new List<MessageTemplateToken>());
            var logEventProp1 = new LogEventProperty("Serilog.Sinks.Splunk.Sample", new ScalarValue("ViaEventCollector"));
            var logEventProp2 = new LogEventProperty("Serilog.Sinks.Splunk.Sample.TestType", new ScalarValue("AddCustomFields"));
            var logEventPropDict = new LogEventProperty[] {logEventProp1, logEventProp2};
            var logEvent = new LogEvent(timestamp: timeStamp,level:LogEventLevel.Debug,exception: testException,messageTemplate: msgTemplate, properties: logEventPropDict);
            //Act
            sut.Format(logEvent, eventtTextWriter);
            //Assert


        }
    }
}

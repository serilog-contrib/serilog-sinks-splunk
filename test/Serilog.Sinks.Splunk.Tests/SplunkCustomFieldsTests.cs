using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.Splunk;
using NUnit;
using NUnit.Framework;


namespace Serilog.Sinks.Splunk.Tests
{
    
    class SplunkCustomFieldsTests
    {
        const string SPLUNK_FULL_ENDPOINT = "http://ws2012-devops:8088/services/collector";
        //  Patrik local vm machine   

        const string SPLUNK_ENDPOINT = "http://ws2012-devops:8088"; //  Patrik local vm machine    
        const string SPLUNK_HEC_TOKEN = "1AFAC088-BFC6-447F-A358-671FA7465342"; // PATRIK lOCAL VM -MACHINE
        [Test]
        void Test_Add_Extra_Splunk_Field_()
        {
            //Arrange
            var metaData = new CustomFields();
            metaData.CustomFieldList.Add(new CustomField("relChan", "Test"));
            metaData.CustomFieldList.Add(new CustomField("version", "17.8.9.beta"));
            metaData.CustomFieldList.Add(new CustomField("rel", "REL1706"));
            metaData.CustomFieldList.Add(new CustomField("role",new List<string>() { "service", "rest", "ESB" }));

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

            //Assert

        }
    }
}

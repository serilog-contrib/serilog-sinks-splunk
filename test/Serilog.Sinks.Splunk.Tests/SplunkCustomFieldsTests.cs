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
            var releaseChannel = new CustomField("relChan","Test");
            var metaData = new CustomFields();
            var sut = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.EventCollector(
        splunkHost: SPLUNK_ENDPOINT
        , eventCollectorToken: SPLUNK_HEC_TOKEN
        , host: System.Environment.MachineName
        , source: "BackPackTestServerChannel"
        , sourceType: "_json")
    .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
    .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "AddExtraFields")
    .CreateLogger();
            //Act

            //Assert

        }
    }
}

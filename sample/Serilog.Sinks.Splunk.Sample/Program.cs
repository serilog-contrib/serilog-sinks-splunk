using System;
using System.Linq;
using System.Net;
using Splunk.Client;


namespace Serilog.Sinks.Splunk.Sample
{
    class Program
    {
        public static string EventCollectorToken = "DC279305-1816-44D6-9D7A-6CBB70F0A049";

        static void Main(string[] args)
        {
            var stub = new Stub();
;
            var tcp = new ViaTcp();
            var udp = new ViaUdp();
            var ec = new ViaEventCollector();
            var eco = new ViaEventCollectorWithExtendedOptions();

            eco.Configure();
            //ec.Configure();
            //udp.Configure();
            //tcp.Configure();

            Log.Information("Simulation running, press any key to exit.");

            stub.Run();

            var range = Enumerable.Range(0, 100);

            foreach (var i in range)
            {
                Log.Information("Say hello to {0}", i);
            }

            Console.ReadLine();
        }
    }

    class ViaEventCollector : IConfigure
    {
         public void Configure()
         {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Log.Logger = new LoggerConfiguration()
                 .WriteTo.LiterateConsole()
                 .WriteTo.SplunkViaEventCollector("https://mysplunk:8088/services/collector", Program.EventCollectorToken,
                    renderTemplate:false,
                    batchSizeLimit:150,
                    batchIntervalInSeconds:5)
                 .Enrich.WithThreadId()
                 .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                 .MinimumLevel.Debug()
                 .CreateLogger();
         }
    }

    class ViaEventCollectorWithExtendedOptions : IConfigure
    {
        public void Configure()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Log.Logger = new LoggerConfiguration()
                 .WriteTo.LiterateConsole()
                 .WriteTo.SplunkViaEventCollector("https://mysplunk:8088/services/collector", 
                    Program.EventCollectorToken,
                    "Serilog",
                    "",
                   Environment.MachineName,
                    "" ,
                    renderTemplate: false)
                 .Enrich.WithThreadId()
                 .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                 .MinimumLevel.Debug()
                 .CreateLogger();
        }
    }


    class ViaTcp : IConfigure
    {
        public void Configure()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.LiterateConsole()
            .WriteTo.SplunkViaTcp("127.0.0.1", 10001, renderTemplate: false)
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaTCP")
            .MinimumLevel.Debug()
            .CreateLogger();
        }
    } 

    class ViaUdp : IConfigure
    {
        public void Configure()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.LiterateConsole()
            .WriteTo.SplunkViaUdp("127.0.0.1", 10002, renderTemplate: false)
            .Enrich.WithThreadId()
            .Enrich.WithProperty("SplunkSample", "ViaUDP")
            .MinimumLevel.Debug()
            .CreateLogger();
        }
    }


}

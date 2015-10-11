using System;
using System.Net;
using Splunk.Client;


namespace Serilog.Sinks.Splunk.Sample
{
    class Program
    {
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
                 .WriteTo.SplunkViaEventCollector("https://mysplunk:8088/services/collector", "685546AE-0278-4786-97C4-5971676D5D70",renderTemplate:false)
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
                    "685546AE-0278-4786-97C4-5971676D5D70",
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

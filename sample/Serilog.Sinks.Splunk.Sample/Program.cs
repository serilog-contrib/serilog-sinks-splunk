using System;
using Splunk.Client;


namespace Serilog.Sinks.Splunk.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var stub = new Stub();

            var http = new ViaHttp();
            var tcp = new ViaTcp();
            var udp = new ViaHttp();

            http.Configure();
            udp.Configure();
            tcp.Configure();

            Log.Information("Simulation running, press any key to exit.");

            stub.Run();

            Console.ReadLine();
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
            .Enrich.WithProperty("SplunkSample", "ViaTCP")
            .MinimumLevel.Debug()
            .CreateLogger();
        }
    }

    class ViaHttp : IConfigure
    {
        public void Configure()
        {
            var generalSplunkContext = new global::Splunk.Client.Context(Scheme.Https, "127.0.0.1", 8089);

            var transmitterArgs = new TransmitterArgs
            {
                Source = "Splunk.Sample",
                SourceType = "Splunk Sample Source"
            };

            const string username = "my splunk user";
            const string password = "my splunk password";
            const string splunkIndex = "mysplunktest";

            var serilogContext = new SplunkContext(
                generalSplunkContext, 
                splunkIndex, 
                username,
                password, 
                null, 
                transmitterArgs);

            Log.Logger = new LoggerConfiguration()
            .WriteTo.LiterateConsole()
            .WriteTo.SplunkViaHttp(serilogContext, 100, TimeSpan.FromSeconds(10))
            .Enrich.WithThreadId()
            .Enrich.WithProperty("SplunkSample", "ViaHttp")
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

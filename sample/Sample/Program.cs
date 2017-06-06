using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Sinks.Splunk;

namespace Sample
{
    public class Program
    {
        const string SPLUNK_FULL_ENDPOINT = "http://ws2012-devops:8088/services/collector";
            //  Patrik local vm machine   

        const string SPLUNK_ENDPOINT = "http://ws2012-devops:8088"; //  Patrik local vm machine    
        const string SPLUNK_HEC_TOKEN = "1AFAC088-BFC6-447F-A358-671FA7465342"; // PATRIK lOCAL VM -MACHINE
        public static string EventCollectorToken = SPLUNK_HEC_TOKEN; // "2B94855F-1184-46F7-BFF1-56A3112F627E";

        public static void Main(string[] args)
        {
            var eventsToCreate = 100;
            var runSSL = false;

            if (args.Length > 0)
                eventsToCreate = int.Parse(args[0]);

            if (args.Length == 2)
                runSSL = bool.Parse(args[1]);

            Log.Information("Sample starting up");
            Serilog.Debugging.SelfLog.Enable(System.Console.Out);

            //UsingHostOnly(eventsToCreate);
            UsingFullUri(eventsToCreate);
            //OverridingSource(eventsToCreate);
            //OverridingSourceType(eventsToCreate);
            //OverridingHost(eventsToCreate);
            //WithNoTemplate(eventsToCreate);
            //WithCompactSplunkFormatter(eventsToCreate);
            //if (runSSL)
            //{
            //    UsingSSL(eventsToCreate);
            //}
            AddExtraFields(eventsToCreate);

            Log.Debug("Done");
        }

        private static void WithCompactSplunkFormatter(int eventsToCreate)
        {
            // Vanilla Test with full uri specified
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_FULL_ENDPOINT,
                    Program.EventCollectorToken, new CompactSplunkJsonFormatter())
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType",
                    "Vanilla with CompactSplunkJsonFormatter specified")
                .CreateLogger();


            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("{Counter}{Message}", i, "Running vanilla loop with CompactSplunkJsonFormatter");
            }

            Log.CloseAndFlush();
        }

        public static void OverridingSource(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    source: "Serilog.Sinks.Splunk.Sample.TestSource")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Override")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source override loop {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void OverridingSourceType(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    sourceType: "_json")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Type Override")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source type override loop {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void OverridingHost(int eventsToCreate)
        {
            // Override Host
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    host: "myamazingmachine")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Host Override")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running host override loop {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void UsingFullUri(int eventsToCreate)
        {
            // Vanilla Test with full uri specified
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_FULL_ENDPOINT,
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Vanilla with full uri specified")
                .CreateLogger();


            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running vanilla loop with full uri {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void UsingHostOnly(int eventsToCreate)
        {
            // Vanilla Tests just host
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Vanilla No services/collector in uri")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running vanilla without host name and port only {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void WithNoTemplate(int eventsToCreate)
        {
            // No Template
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    renderTemplate: false)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "No Templates")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running no template loop {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        public static void UsingSSL(int eventsToCreate)
        {
            // SSL
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "HTTPS")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("HTTPS {Counter}", i);
            }
            Log.CloseAndFlush();
        }

        public static void AddExtraFields(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector(
                    splunkHost: SPLUNK_ENDPOINT
                    , eventCollectorToken: SPLUNK_HEC_TOKEN
                    , host: System.Environment.MachineName
                    , source: "BackPackTestServerChannel"
                    , sourceType: "_json")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "AddExtraFields")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("AddExtraFields {Counter}", i);
            }

            Log.CloseAndFlush();
        }
    }

   
}

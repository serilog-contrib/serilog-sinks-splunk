using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Splunk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    public class Program
    {
        const string SPLUNK_FULL_ENDPOINT = "http://splunk:8088/services/collector/event"; // Full splunk url 
        const string SPLUNK_ENDPOINT = "http://splunk:8088"; // Your splunk url  
        const string SPLUNK_HEC_TOKEN = "00112233-4455-6677-8899-AABBCCDDEEFF"; // Your HEC token. See http://docs.splunk.com/Documentation/Splunk/latest/Data/UsetheHTTPEventCollector
        public static string EventCollectorToken = SPLUNK_HEC_TOKEN;

        public static async Task Main(string[] args)
        {
            // Bootstrap a simple logger.
            var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                logger.Information("Sample app starting up...");
                logger.Information("Startup arguments \"{Arguments}\".", args);

                var eventsToCreate = 10;
                var runSSL = false;
                var secToWait = 30;

                if (args.Length > 0)
                    eventsToCreate = int.Parse(args[0]);

                if (args.Length == 2)
                    runSSL = bool.Parse(args[1]);

                if (args.Length == 3)
                    secToWait = int.Parse(args[2]);

                Serilog.Debugging.SelfLog.Enable(msg =>
                {
                    Console.WriteLine(msg);
                    throw new Exception("Failed to write to Serilog.", new Exception(msg));
                });


                while (secToWait-- > 0)
                {
                    logger.Information("Waiting {secToWait} seconds...", secToWait);
                    await Task.Delay(1000);
                }

                logger.Information("Creating logger {MethodName}.", nameof(OverridingSubsecondPrecisionMicroseconds));
                OverridingSubsecondPrecisionMicroseconds(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(OverridingSubsecondPrecisionNanoseconds));
                OverridingSubsecondPrecisionNanoseconds(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(UsingAppSettingsJson));
                UsingAppSettingsJson(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(UsingHostOnly));
                UsingHostOnly(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(UsingFullUri));
                UsingFullUri(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(OverridingSource));
                OverridingSource(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(OverridingSourceType));
                OverridingSourceType(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(OverridingHost));
                OverridingHost(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(WithNoTemplate));
                WithNoTemplate(eventsToCreate);

                logger.Information("Creating logger {MethodName}.", nameof(WithCompactSplunkFormatter));
                WithCompactSplunkFormatter(eventsToCreate);

                if (runSSL)
                {
                    logger.Information("Creating logger {MethodName}.", nameof(UsingSSL));
                    UsingSSL(eventsToCreate);
                }

                logger.Information("Creating logger {MethodName}.", nameof(AddCustomFields));
                AddCustomFields(eventsToCreate);
            }
            finally
            {
                logger.Information("Done...");
                Log.CloseAndFlush();
            }
        }

        public static void UsingAppSettingsJson(int eventsToCreate)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running via appsettings.json {Counter}", i);
            }

            Log.CloseAndFlush();
        }

        private static void WithCompactSplunkFormatter(int eventsToCreate)
        {
            // Vanilla Test with full uri specified
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.EventCollector(
                    SPLUNK_FULL_ENDPOINT,
                    Program.EventCollectorToken, new CompactSplunkJsonFormatter())
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType",
                    "Vanilla with CompactSplunkJsonFormatter specified")
                .CreateLogger();


            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("{Counter}{Message}", i, " Running vanilla loop with CompactSplunkJsonFormatter");
            }

            Log.CloseAndFlush();
        }

        public static void OverridingSubsecondPrecisionMicroseconds(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    subSecondPrecision: SubSecondPrecision.Microseconds)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Override")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source override loop {Counter} subseconds: {subsecondPrecision}", i, SubSecondPrecision.Microseconds.ToString());
            }

            Log.CloseAndFlush();
        }

        public static void OverridingSubsecondPrecisionNanoseconds(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken,
                    subSecondPrecision: SubSecondPrecision.Nanoseconds)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Override")
                .CreateLogger();

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source override loop {Counter} subseconds: {subsecondPrecision}", i, SubSecondPrecision.Nanoseconds.ToString());
            }

            Log.CloseAndFlush();
        }

        public static void OverridingSource(int eventsToCreate)
        {
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
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
                .WriteTo.Console()
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
                .WriteTo.Console()
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
                .WriteTo.Console()
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
                .WriteTo.Console()
                .WriteTo.EventCollector(
                    SPLUNK_ENDPOINT,
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Vanilla No services/collector/event in uri")
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
                .WriteTo.Console()
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
                .WriteTo.Console()
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

        public static void AddCustomFields(int eventsToCreate)
        {
            var metaData = new CustomFields(new List<CustomField>
            {
                new CustomField("relChan", "Test"),
                new CustomField("version", "17.8.9.beta"),
                new CustomField("rel", "REL1706"),
                new CustomField("role", new List<string>() { "service", "rest", "ESB" })
            });
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
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

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("AddCustomFields {Counter}", i);
            }

            Log.CloseAndFlush();
        }
    }


}

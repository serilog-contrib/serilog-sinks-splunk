using System.Linq;
using System.Threading;
using Serilog;
using Serilog.Core;

namespace Sample
{
    public class Program
    {
        public static string EventCollectorToken = "1A4D65C9-601A-4717-AD6C-E1EC36A46B69";
        
        public static void Main(string[] args)
        {
            var eventsToCreate = 100;
            
            if(args.Length > 0)
                eventsToCreate = int.Parse(args[0]); 
            
            Log.Information("Sample starting up");
            Serilog.Debugging.SelfLog.Out = System.Console.Out;

            // Vanilla Tests just host
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "http://localhost:8088",
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Vanilla No services/collector")
                .CreateLogger(); 

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running vanilla without extended endpoint loop {Counter}", i);
            }

            Log.CloseAndFlush();

            // Vanilla Test with full uri specified
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "http://localhost:8088/services/collector",
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Vanilla with full uri specified")
                .CreateLogger(); 

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running vanilla loop {Counter}", i);
            }

            Log.CloseAndFlush();

            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "http://localhost:8088", 
                    Program.EventCollectorToken,
                    source: "Serilog.Sinks.Splunk.Sample")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Override")
                .CreateLogger();
            
            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source override loop {Counter}", i);
            }

            Log.CloseAndFlush();
            
            // Override Host
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "http://localhost:8088", 
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

             // No Template
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "http://localhost:8088", 
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

            // // SSL
            // Log.Logger = new LoggerConfiguration()
            //     .MinimumLevel.Debug()
            //     .WriteTo.LiterateConsole() 
            //     .WriteTo.EventCollector(
            //         "https://localhost:8088",
            //         Program.EventCollectorToken)
            //     .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
            //     .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "HTTPS")
            //     .CreateLogger(); 
            
            // foreach (var i in Enumerable.Range(0, eventsToCreate))
            // {
            //     Log.Information("HTTPS {Counter}", i);
            // }
            // Log.CloseAndFlush();

            Log.Debug("Done");
                 
        }
    }
}

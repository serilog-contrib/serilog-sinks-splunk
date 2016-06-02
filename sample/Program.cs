using System.Linq;
using System.Threading;
using Serilog;
using Serilog.Core;

namespace Sample
{
    public class Program
    {
        public static string EventCollectorToken = "D7E04CDB-71A8-4266-90A1-90EF1620AA4B";
        
        public static void Main(string[] args)
        {
            var eventsToCreate = 100;
            
            if(args.Length > 0)
                eventsToCreate = int.Parse(args[0]); 
            
            Log.Information("Sample starting up");

            // Vanilla Tests
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.SplunkViaEventCollector(
                    "http://localhost:8088/services/collector",
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestCase", "Vanilla")
                .CreateLogger(); 

            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running vanilla loop {Counter}", i); 
                Thread.Sleep(5); 
            }
            
            // Override Source
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.SplunkViaEventCollector(
                    "http://localhost:8088/services/collector", 
                    Program.EventCollectorToken,
                    source: "Serilog.Sinks.Splunk.Sample")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Source Override")
                .CreateLogger();
            
            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running source override loop {Counter}", i); 
                Thread.Sleep(5);
            }
            
            // Override Host
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.SplunkViaEventCollector(
                    "http://localhost:8088/services/collector", 
                    Program.EventCollectorToken,
                    host: "myamazingmachine")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "Host Override")
                .CreateLogger();
            
            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running host override loop {Counter}", i); 
                Thread.Sleep(5);
            } 
            
             // No Template
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.SplunkViaEventCollector(
                    "http://localhost:8088/services/collector", 
                    Program.EventCollectorToken,
                    renderTemplate: false)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "No Templates")
                .CreateLogger();
            
            foreach (var i in Enumerable.Range(0, eventsToCreate))
            {
                Log.Information("Running no template loop {Counter}", i); 
                Thread.Sleep(5);
            }
            
            Log.Debug("Waiting for Events to Flush");
            Thread.Sleep(5000);
            Log.Debug("Done");
                 
        }
    }
}

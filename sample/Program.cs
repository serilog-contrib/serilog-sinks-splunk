using System.Linq;
using System.Threading;
using Serilog;
using Serilog.Core;

namespace Sample
{
    public class Program
    {
        public static string EventCollectorToken = "04B42E81-100E-4BED-8AE9-FC5EE4E08602";
        
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
                Thread.Sleep(5); 
            }

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
                Thread.Sleep(5); 
            }

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
                Thread.Sleep(5);
            }
            
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
                Thread.Sleep(5);
            } 
            
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
                Thread.Sleep(5);
            }

            // SSL
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole() 
                .WriteTo.EventCollector(
                    "https://localhost:8088",
                    Program.EventCollectorToken)
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample", "ViaEventCollector")
                .Enrich.WithProperty("Serilog.Sinks.Splunk.Sample.TestType", "HTTPS")
                .CreateLogger(); 
            
            Log.Debug("Waiting for Events to Flush");
            Thread.Sleep(5000);
            Log.Debug("Done");
                 
        }
    }
}

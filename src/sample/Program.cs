using System;
using System.Linq;
using System.Net;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Splunk;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string splunkHost = "https://192.168.71.1:8088";
            string splunkEventCollectorToken = "274AD921-FB85-429B-B09E-4EE069843218";
         

            Log.Logger = new LoggerConfiguration() 
                .WriteTo.LiterateConsole()
                .WriteTo.EventCollector (splunkHost, splunkEventCollectorToken)
                .CreateLogger();

            Serilog.Debugging.SelfLog.Out = Console.Out;

            Log.Information("Sample starting up");

            foreach (var i in Enumerable.Range(0, 1000))
            {
                Log.Information("Running loop {Counter}", i);

                Thread.Sleep(1000);
                Log.Debug("Loop iteration done");
            }

            Console.ReadLine();
        }
    }
}

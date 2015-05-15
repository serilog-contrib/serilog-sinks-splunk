using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Serilog.Sinks.Splunk.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.SplunkViaTcp("127.0.0.1", 10001)
                .CreateLogger();

            Log.Information("Just another test");

            Console.ReadLine();
        }
    }
}

using System.Linq;
using System.Threading;
using Serilog;
using Serilog.Core;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        { 

            Log.Logger = new LoggerConfiguration() 
                .WriteTo.LiterateConsole() 
                .CreateLogger();

            Log.Information("Sample starting up");

            foreach (var i in Enumerable.Range(0, 1000))
            {
                Log.Information("Running loop {Counter}", i);

                Thread.Sleep(1000);
                Log.Debug("Loop iteration done");
            }
        }
    }
}

using System.Collections.Concurrent;
using System.Linq;
using Serilog.Generator.Actors;
using Serilog.Generator.Model;

namespace Serilog.Sinks.Splunk.Sample
{
    internal class Stub
    {
        public void Run()
        {
            const int initialCustomers = 1;

            Log.Information("Simulation starting with {InitialCustomers} initial customers...", initialCustomers);

            var catalog = new Catalog();

            var customers = new ConcurrentBag<Customer>(Enumerable.Range(0, initialCustomers)
                .Select(_ => new Customer(catalog)));

            var traffic = new TrafficReferral(customers, catalog);
            var admin = new Administrator(catalog);

            foreach (var c in customers)
                c.Start();

            admin.Start();
            traffic.Start();
        }
    }
}
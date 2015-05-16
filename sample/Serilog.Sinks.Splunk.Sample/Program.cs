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
                .WriteTo.SplunkViaTcp("127.0.0.1", 10001, renderTemplate:true)
                .CreateLogger();

            var person = new Person() {DateOfBirth = DateTime.Now.AddYears(-30), FirstName = "Joe", Surname = "Bloggs"};

            Log.Information("Just another test {@person}", person);

            Console.ReadLine();
        }
    }

    internal class Person
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

    }

}

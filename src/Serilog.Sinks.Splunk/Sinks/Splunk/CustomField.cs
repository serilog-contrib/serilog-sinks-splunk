using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk
{
    public class CustomField
    {
        public string Name { get; set; }
        public List<string> ValueList { get; set; }
        public CustomField(string name, string value)
        {
            Name = name;
            ValueList = new List<string> { value };
        }

        public CustomField(string name, List<string> value)
        {
            Name = name;
            ValueList = value;
        }
    }
}

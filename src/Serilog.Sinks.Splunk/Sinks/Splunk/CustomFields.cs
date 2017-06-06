using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk.Sinks.Splunk
{
    public class CustomField 
    {
        public string Name { get; set; }
        public List<string> ValueList { get; set; }
        public CustomField(string name, string value)
        {
            Name = name;
            ValueList = new List<string> {value};
        }

        public CustomField(string name, List<string> value)
        {
            Name = name;
            ValueList = value;
        }
    }
    public class CustomFields
    {
        public List<CustomField> CustomFieldList { get; set; }
        public CustomFields()
        {
            CustomFieldList = new List<CustomField>();
        }
        public CustomFields(CustomField customField)
        {
            CustomFieldList = new List<CustomField>{customField};
        }
        public CustomFields(List<CustomField> customFields)
        {
            CustomFieldList = customFields  ;
        }
    }
}

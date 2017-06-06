using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk
{
   
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

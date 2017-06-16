using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Class for storing CustomFields that splunk will index for the event but will not be displayed in the event.
    /// They are the same for all events. Could could contain type of server or releasecode see: http://dev.splunk.com/view/event-collector/SP-CAAAFB6
    /// </summary>
    public class CustomFields
    {/// <summary>
    /// The List of all CustomFields of type CustomField
    /// </summary>
        public List<CustomField> CustomFieldList { get; set; }
        /// <summary>
        /// Constructor with no inital data
        /// </summary>
        public CustomFields()
        {
            CustomFieldList = new List<CustomField>();
        }
        /// <summary>
        /// Constructor with simple CustomField
        /// </summary>
        /// <param name="customField"></param>
        public CustomFields(CustomField customField)
        {
            CustomFieldList = new List<CustomField>{customField};
        }
        /// <summary>
        /// Constructor with a list of CustomFields
        /// </summary>
        public CustomFields(List<CustomField> customFields)
        {
            CustomFieldList = customFields  ;
        }
    }
}

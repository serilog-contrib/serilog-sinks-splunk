using System.Collections.Generic;

namespace Serilog.Sinks.Splunk
{ /// <summary>
/// A Class for storing CustomField. They are sort of key,value pair. In simpler form key as string and value as single string, but could also be key and list of strings.
/// </summary>
    public class CustomField
    { /// <summary>
    /// the fieldsname eg: role, version,
    /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// All values even simple string are stored as a list
        /// </summary>
        public List<string> ValueList { get; set; }
        /// <summary>
        /// constructor for a simple fieldname and a value both are strings
        /// </summary>
        /// <param name="name">Name of filed to be indexed by Splunk. Eg Role,Version,Channel</param>
        /// <param name="value">Value of keypair. Eg. Test,1.08, RestService</param>
        public CustomField(string name, string value)
        {
            Name = name;
            ValueList = new List<string> { value };
        }
        /// <summary>
        /// Constructor for Name and array of values
        /// </summary>
        /// <param name="name">Name of field eg TypeOfResource</param>
        /// <param name="value">Array of values that should be connected with field.Eg Backend,Service,Rest</param>
        public CustomField(string name, List<string> value)
        {
            Name = name;
            ValueList = value;
        }
    }
}

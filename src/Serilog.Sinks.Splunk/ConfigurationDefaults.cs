namespace Serilog
{
    internal static class ConfigurationDefaults
    {
        internal const string DefaultSource = "";
        internal const string DefaultSourceType = "";
        internal const string DefaultHost = "";
        internal const string DefaultIndex = "";

        /// <summary>
        /// The default HTTP Event Collector path when not set via configuration.
        /// </summary>
        /// <remarks>
        /// https://docs.splunk.com/Documentation/Splunk/9.1.0/Data/UsetheHTTPEventCollector#Send_data_to_HTTP_Event_Collector_on_Splunk_Enterprise
        /// </remarks>
        internal const string DefaultEventCollectorPath = "services/collector/event";
        internal const string DefaultCollectorPath = "services/collector";
    }
}

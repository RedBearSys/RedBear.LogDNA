using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Data structure for data sent to LogDNA servers.
    /// </summary>
    public class BufferMessage
    {
        /// <summary>
        /// Gets the type of the LogDNA object.
        /// </summary>
        /// <value>
        /// Value is always "ls".
        /// </value>
        [JsonProperty("e")]
        public string LogObjectType => "ls";

        /// <summary>
        /// Gets the log lines.
        /// </summary>
        /// <value>
        /// The log lines.
        /// </value>
        [JsonProperty("ls")]
        public IEnumerable<LogLine> LogLines { get; set; }
    }
}

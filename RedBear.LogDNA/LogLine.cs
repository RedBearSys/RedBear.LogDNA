using System;
using Newtonsoft.Json;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Represents a single line of a log file.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class LogLine
    {
        /// <summary>
        /// Gets the type of the LogDNA object.
        /// </summary>
        /// <value>
        /// Value is always "l".
        /// </value>
        [JsonProperty("e")]
        public string LogObjectType => "l";

        /// <summary>
        /// Gets or sets the timestamp of the log line.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        [JsonProperty("t")]
        public int Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the content of the line.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("l")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the filename from which the log line originates. In many cases, this won't be a file but a process name.
        /// </summary>
        /// <value>
        /// The filename or process name.
        /// </value>
        [JsonProperty("f")]
        public string Filename { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLine"/> class using the current UTC date and time as the timestamp.
        /// </summary>
        /// <param name="logName">Name of the log. This will be used as the 'filename'.</param>
        /// <param name="content">The content of the log line.</param>
        public LogLine(string logName, string content)
        {
            Timestamp = DateTime.UtcNow.ToUnixTimestamp();
            Content = content.Length > 32000 ? content.Substring(0, 32000) : content;
            Filename = logName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLine"/> class.
        /// </summary>
        /// <param name="logName">Name of the log. This will be used as the 'filename'.</param>
        /// <param name="content">The content of the log line.</param>
        /// <param name="utc">The UTC date and time to associate with this log line.</param>
        public LogLine(string logName, string content, DateTime utc)
        {
            Timestamp = utc.ToUnixTimestamp();
            Content = content.Length > 32000 ? content.Substring(0, 32000) : content;
            Filename = logName;
        }
    }
}

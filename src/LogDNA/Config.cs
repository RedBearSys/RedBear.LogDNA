using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Contains runtime settings for the ApiClient.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        public enum TransportType
        {
            Http,
            WebSocket
        }

        /// <summary>
        /// Gets the transport type. Only websockets is supported.
        /// </summary>
        /// <value>
        /// The Websocket transport type.
        /// </value>
        public TransportType Transport => TransportType.WebSocket;
        /// <summary>
        /// Gets or sets the delay before reconnecting after an authentication failure (in milliseconds).
        /// </summary>
        /// <value>
        /// The authentication fail delay. Defaults to 15 minutes.
        /// </value>
        public int AuthFailDelay { get; set; }
        /// <summary>
        /// Gets or sets the period of time after which a flush should happen automatically.
        /// </summary>
        /// <value>
        /// The flush interval. Defaults to 250ms.
        /// </value>
        public int FlushInterval { get; set; }
        /// <summary>
        /// Gets or sets the number of buffer entries after which a flush should happen automatically.
        /// </summary>
        /// <value>
        /// The flush limit.
        /// </value>
        public int FlushLimit { get; set; }
        /// <summary>
        /// Gets or sets the maximum size of the log line buffer.
        /// </summary>
        /// <value>
        /// The buffer limit. Defaults to 10,000 entries.
        /// </value>
        public int BufferLimit { get; set; }
        /// <summary>
        /// Gets or sets the LogDNA key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string IngestionKey { get; set; }
        /// <summary>
        /// Gets or sets the tags used for dynamic grouping.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets all the tags in a comma-separated string.
        /// </summary>
        /// <value>
        /// All tags.
        /// </value>
        [JsonProperty("tags")]
        public string AllTags => Tags.Any() ? string.Join(",", Tags) : null;

        /// <summary>
        /// Gets the name of the host. By default, this is the machine name, but it can be overridden.
        /// </summary>
        /// <value>
        /// The host name.
        /// </value>
        [JsonProperty("hostname")]
        public string HostName { get; set; }

        /// <summary>
        /// Gets the MAC address.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("mac")]
        public string MacAddress => null;

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("ip")]
        public string IpAddress => null;

        /// <summary>
        /// Gets the name of this agent.
        /// </summary>
        /// <value>
        /// "RedBear.LogDNA"
        /// </value>
        [JsonProperty("agentname")]
        public string AgentName => "RedBear.LogDNA";

        /// <summary>
        /// Gets the agent version.
        /// </summary>
        /// <value>
        /// The agent version.
        /// </value>
        [JsonProperty("agentversion")]
        public string AgentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Gets the Operating System ID.
        /// </summary>
        /// <value>
        /// "win32"
        /// </value>
        [JsonProperty("osdist")]
        public string OsDist => "win32";

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("awsid")]
        public string AwsId => null;

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("awsregion")]
        public string AwsRegion => null;

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("awsaz")]
        public string AwsAz => null;

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("awsami")]
        public string AwsAmi => null;

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        [JsonProperty("awstype")]
        public string AwsType => null;

        /// <summary>
        /// Gets the user agent to be used when communicating with LogDNA servers.
        /// </summary>
        /// <value>
        /// The user agent: "AgentName/AgentVersion"
        /// </value>
        public string UserAgent => $"{AgentName}/{AgentVersion}";

        /// <summary>
        /// Gets or sets the authentication token received from LogDNA.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets hostname of the log server.
        /// </summary>
        /// <value>
        /// The log server.
        /// </value>
        public string LogServer { get; set; }

        /// <summary>
        /// Gets or sets the port number of the log server.
        /// </summary>
        /// <value>
        /// The log server port.
        /// </value>
        public int LogServerPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an SSL connection to the log server should be used.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use a secure websocket connection; otherwise, <c>false</c>.
        /// </value>
        public bool LogServerSsl { get; set; }

        public Config(string ingestionIngestionKey)
        {
            IngestionKey = ingestionIngestionKey;
            AuthFailDelay = 900000;
            FlushInterval = 250;
            FlushLimit = 5000;
            BufferLimit = 10000;
            Tags = new List<string>();
            HostName = Environment.MachineName;
        }
    }
}

using System.Collections.Generic;

namespace RedBear.LogDNA
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the transport type.
        /// </summary>
        /// <value>
        /// The transport type.
        /// </value>
        ConfigurationManager.TransportType Transport { get; set; }

        /// <summary>
        /// Gets or sets the delay before reconnecting after an authentication failure (in milliseconds).
        /// </summary>
        /// <value>
        /// The authentication fail delay. Defaults to 15 minutes.
        /// </value>
        int AuthFailDelay { get; set; }

        /// <summary>
        /// Gets or sets the period of time after which a flush should happen automatically.
        /// </summary>
        /// <value>
        /// The flush interval. Defaults to 250ms.
        /// </value>
        int FlushInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of buffer entries after which a flush should happen automatically.
        /// </summary>
        /// <value>
        /// The flush limit.
        /// </value>
//public int FlushLimit { get; set; }
        /// <summary>
        /// Gets or sets the maximum size of the log line buffer.
        /// </summary>
        /// <value>
        /// The buffer limit. Defaults to 10,000 entries.
        /// </value>
//public int BufferLimit { get; set; }
        /// <summary>
        /// Gets or sets the LogDNA key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        string IngestionKey { get; set; }

        /// <summary>
        /// Gets or sets the tags used for dynamic grouping.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets all the tags in a comma-separated string.
        /// </summary>
        /// <value>
        /// All tags.
        /// </value>
        string AllTags { get; }

        /// <summary>
        /// Gets the name of the host. By default, this is the machine name, but it can be overridden.
        /// </summary>
        /// <value>
        /// The host name.
        /// </value>
        string HostName { get; set; }

        /// <summary>
        /// Gets the MAC address.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string MacAddress { get; }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string IpAddress { get; }

        /// <summary>
        /// Gets the name of this agent.
        /// </summary>
        /// <value>
        /// "RedBear.LogDNA"
        /// </value>
        string AgentName { get; }

        /// <summary>
        /// Gets the agent version.
        /// </summary>
        /// <value>
        /// The agent version.
        /// </value>
        string AgentVersion { get; }

        /// <summary>
        /// Gets the Operating System ID.
        /// </summary>
        /// <value>
        /// "win32"
        /// </value>
        string OsDist { get; }

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string AwsId { get; }

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string AwsRegion { get; }

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string AwsAz { get; }

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string AwsAmi { get; }

        /// <summary>
        /// Gets an Amazon Web Services value. Not used.
        /// </summary>
        /// <value>
        /// NULL. Not used.
        /// </value>
        string AwsType { get; }

        /// <summary>
        /// Gets the user agent to be used when communicating with LogDNA servers.
        /// </summary>
        /// <value>
        /// The user agent: "AgentName/AgentVersion"
        /// </value>
        string UserAgent { get; }

        /// <summary>
        /// Gets or sets the authentication token received from LogDNA.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets hostname of the log server.
        /// </summary>
        /// <value>
        /// The log server.
        /// </value>
        string LogServer { get; set; }

        /// <summary>
        /// Gets or sets the port number of the log server.
        /// </summary>
        /// <value>
        /// The log server port.
        /// </value>
        int LogServerPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an SSL connection to the log server should be used.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use a secure websocket connection; otherwise, <c>false</c>.
        /// </value>
        bool LogServerSsl { get; set; }

        /// <summary>
        /// Log the internal operations of the LogDNA client to the Console window.
        /// </summary>
        /// <returns></returns>
        bool LogInternalsToConsole { get; set; }

        /// <summary>
        /// Returns http:// or https:// depending upon whether SSL is to be used.
        /// </summary>
        /// <value>
        /// The HTTP protocol.
        /// </value>
        string HttpProtocol { get; }

        /// <summary>
        /// Gets or sets the time to wait (in ms) before retrying a send operation.
        /// </summary>
        /// <value>
        /// The time to wait (in ms) before retrying a send operation.
        /// </value>
        int RetryTimeout { get; set; }

        /// <summary>
        /// Authenticate and initialise logging.
        /// </summary>
        IApiClient Initialise();
    }
}
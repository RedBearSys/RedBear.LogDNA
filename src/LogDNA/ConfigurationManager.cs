using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Contains runtime settings for the ApiClient.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigurationManager : IConfigurationManager
    {
        public enum TransportType
        {
            Http,
            WebSocket
        }

        /// <summary>
        /// Gets the transport type.
        /// </summary>
        /// <value>
        /// The transport type.
        /// </value>
        public TransportType Transport { get; set; }
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

        /// <summary>
        /// Log the internal operations of the LogDNA client to the Console window.
        /// </summary>
        /// <returns></returns>
        public bool LogInternalsToConsole { get; set; }

        /// <summary>
        /// Returns http:// or https:// depending upon whether SSL is to be used.
        /// </summary>
        /// <value>
        /// The HTTP protocol.
        /// </value>
        public string HttpProtocol => LogServerSsl ? "https://" : "http://";

        /// <summary>
        /// Gets or sets the time to wait (in ms) before retrying a send operation.
        /// </summary>
        /// <value>
        /// The time to wait (in ms) before retrying a send operation.
        /// </value>
        public int RetryTimeout { get; set; } = 5000;

        public ConfigurationManager(string ingestionIngestionKey)
        {
            IngestionKey = ingestionIngestionKey;
            AuthFailDelay = 900000;
            FlushInterval = 250;
            Tags = new List<string>();
            HostName = Environment.MachineName;
            Transport = TransportType.Http;
        }

        /// <summary>
        /// Authenticate and initialise logging.
        /// </summary>
        public IApiClient Initialise()
        {
            var url = new Uri("https://api.logdna.com/authenticate/");
            var status = HttpStatusCode.Unused;
            var tries = 0;

            while (status != HttpStatusCode.OK && tries < 10)
            {
                JObject result;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

                    var response = client.PostAsync(IngestionKey,
                        new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json")).Result;

                    result = response.IsSuccessStatusCode
                        ? JObject.Parse(response.Content.ReadAsStringAsync().Result)
                        : null;

                    status = response.StatusCode;
                }

                if (status != HttpStatusCode.OK)
                {
                    InternalLogger("Auth failed; retry after a delay.");
                    Thread.Sleep(AuthFailDelay);

                    tries++;
                }

                if (status == HttpStatusCode.OK && result?["apiserver"] != null && result["apiserver"].ToString() != url.Host)
                {
                    if (result["ssl"].ToString() == "true")
                    {
                        url = new Uri($"https://{result["apiserver"]}/authenticate/");
                        status = HttpStatusCode.Unused;
                    }
                }

                if (result != null)
                {
                    AuthToken = result["token"].ToString();
                    LogServer = result["server"].ToString();
                    LogServerPort = result["port"].Value<int>();
                    LogServerSsl = result["ssl"].Value<bool>();

                    if (result["transport"].Value<string>() == "http")
                        Transport = TransportType.Http;
                }
            }

            return Transport == TransportType.WebSocket
                ? (IApiClient) new SocketApiClient(this)
                : new HttpApiClient(this);
        }
        
        private void InternalLogger(string message)
        {
            if (LogInternalsToConsole)
                Console.WriteLine(message);
        }
    }
}

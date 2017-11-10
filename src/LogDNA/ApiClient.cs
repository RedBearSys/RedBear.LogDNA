using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedBear.LogDNA.Properties;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Main class for communicating with the LogDNA servers.
    /// </summary>
    public class ApiClient : IApiClient
    {
        public Config Config;
        private JObject _result;
        private WebSocket _ws;
        private int _connectionAttempt;
        
        public LogLineBuffer Buffer { get; set; }
        public bool Active { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Connects to the LogDNA servers using the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public async Task ConnectAsync(Config config)
        {
            Config = config;
            Buffer = new LogLineBuffer(this);
            var url = new Uri("https://api.logdna.com/authenticate/");
            var status = await PostDataAsync(url, Config);

            if (status == HttpStatusCode.OK && _result?["apiserver"] != null && _result["apiserver"].ToString() != url.Host)
            {
                if (_result["ssl"].ToString() == "true")
                {
                    url = new Uri($"https://{_result["apiserver"]}/authenticate/");
                    status = await PostDataAsync(url, Config);
                }
            }

            if (status != HttpStatusCode.OK)
            {
                if (_result != null) Trace.WriteLine(_result.ToString());
                Trace.WriteLine("Auth failed; retry after a delay.");
                await Task.Delay(Config.AuthFailDelay);
                await ConnectAsync(Config);
                return;
            }

            if (_result != null)
            {
                Config.AuthToken = _result["token"].ToString();
                Config.LogServer = _result["server"].ToString();
                Config.LogServerPort = _result["port"].Value<int>();
                Config.LogServerSsl = _result["ssl"].Value<bool>();

                ConnectSocket();
            }
        }

        /// <summary>
        /// Connects to the LogDNA websocket server.
        /// </summary>
        private void ConnectSocket()
        {
            try
            {
                if (_ws != null)
                {
                    try
                    {
                        Active = false;
                        _ws.Close();
                    }
                    catch (Exception)
                    {
                        // Do Nothing
                    }
                }

                Active = true;

                var protocol = "ws";
                if (Config.LogServerSsl) protocol += "s";

                _ws =
                    new WebSocket(
                        $"{protocol}://{Config.LogServer}:{Config.LogServerPort}/?auth_token={WebUtility.UrlEncode(Config.AuthToken)}&timestamp={DateTime.Now.ToJavaTimestamp()}&compress=1&tailmode=&transport=")
                    {
                        Compression = CompressionMethod.Deflate
                    };

                _ws.OnOpen += Ws_OnOpen;
                _ws.OnClose += Ws_OnClose;
                _ws.OnError += Ws_OnError;
                _ws.OnMessage += Ws_OnMessage;
                _ws.Connect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception while connecting to LogDNA");
                Trace.WriteLine(ex.ToString());
                Reconnect();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Disconnects the client from the LogDNA servers.
        /// </summary>
        public void Disconnect()
        {
            Trace.WriteLine("Disconnecting..");
            Flush();
            Active = false;
            Buffer.Running = false;
            _ws?.Close();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Trace.WriteLine("Message received..");

            try
            {
                var data = JObject.Parse(e.Data);

                switch (data["e"].Value<string>())
                {
                    case "u":
                        // Do Nothing - this library doesn't allow auto-updating.
                        break;
                    case "r":
                        ConnectAsync(Config).Wait();
                        break;
                    default:
                        throw new LogDNAException(Resources.UnknownCommand);
                }
            }
            catch (JsonException ex)
            {
                Trace.WriteLine("Deserialisation exception..");
                Trace.WriteLine(ex.ToString());
                throw new LogDNAException(Resources.InvalidCommand);
            }
        }

        private void Reconnect()
        {
            Trace.WriteLine("Reconnecting..");

            _connectionAttempt += 1;
            var timeout = 1000 * Math.Pow(1.5, _connectionAttempt);
            if (timeout > 5000) timeout = 5000;
            Thread.Sleep((int)timeout);
            ConnectSocket();
        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Trace.WriteLine($"Error received: {e.Message}");
            if (Active)
            {
                Reconnect();
            }
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Trace.WriteLine("Connection closed..");
            if (Active)
            {
                Reconnect();
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Trace.WriteLine("Connecting successfully..");
            Active = true;
            Buffer.Running = true;
            _connectionAttempt = 0;
        }

        private async Task<HttpStatusCode> PostDataAsync(Uri url, Config config)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Add("User-Agent", config.UserAgent);

                var response = await client.PostAsync(config.Key,
                    new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    _result = JObject.Parse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    _result = null;
                }
                return response.StatusCode;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Sends the specified message directly to the websocket.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was transmitted successfully.</returns>
        public bool Send(string message)
        {
            if (_ws.ReadyState == WebSocketState.Open)
            {
                try
                {
                    _ws.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception sending message..");
                    Trace.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Adds a LogLine to the buffer.
        /// </summary>
        /// <param name="line">The line.</param>
        public void AddLine(LogLine line)
        {
            Buffer.AddLine(line);
        }


        /// <inheritdoc />
        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        public void Flush()
        {
            Buffer.Flush();
        }
    }
}

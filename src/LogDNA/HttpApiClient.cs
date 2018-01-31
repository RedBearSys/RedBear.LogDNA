using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Main class for communicating with the LogDNA servers.
    /// </summary>
    public class HttpApiClient : IApiClient
    {
        private readonly ConcurrentQueue<LogLine> _buffer = new ConcurrentQueue<LogLine>();
        private readonly ConcurrentDictionary<string, int> _flags = new ConcurrentDictionary<string, int>();

        internal ConfigurationManager Configuration { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Whether the client is connectead to LogDNA.
        /// </summary>
        /// <returns></returns>
        public bool Active { get; set; }

        /// <summary>
        /// Creates a new HttpApiClient using the specified configuration.
        /// </summary>
        /// <param name="configurationManager">The configuration received from the LogDNA servers</param>
        public HttpApiClient(ConfigurationManager configurationManager)
        {
            Configuration = configurationManager;

            var timer = new Timer(Configuration.FlushInterval);
            timer.Elapsed += _timer_Elapsed;
            timer.Start();
        }

        /// <inheritdoc />
        /// <summary>
        /// Connects to the LogDNA servers using the specified configuration.
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
        }


        /// <inheritdoc />
        /// <summary>
        /// Disconnects the client from the LogDNA servers.
        /// </summary>
        public void Disconnect()
        {
            InternalLogger("Disconnecting..");
            Flush();
            Active = false;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Sends the specified message directly to the websocket.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was transmitted successfully.</returns>
        public bool Send(string message)
        {
            InternalLogger($"Sending message: \"{message}\"..");
            
            using (var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
            using (var client = new HttpClient(handler))
            {
                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"{Configuration.HttpProtocol}{Configuration.LogServer}:{Configuration.LogServerPort}/logs/agent?timestamp={DateTime.Now.ToJavaTimestamp()}&hostname={Configuration.HostName}&mac=&ip=&tags={Configuration.AllTags}&compress=1");
 
                var authBytes = Encoding.ASCII.GetBytes($"x:{Configuration.IngestionKey}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                request.Headers.Add("User-Agent", Configuration.UserAgent);
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Keep-Alive", "60000");

                var jsonBytes = Encoding.UTF8.GetBytes(message);
                var ms = new MemoryStream();
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(jsonBytes, 0, jsonBytes.Length);
                }

                ms.Position = 0;

                var content = new StreamContent(ms);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentEncoding.Add("gzip");
                request.Content = content;
                
                var response = client.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    InternalLogger($"Received HTTP Response: {response.StatusCode}");

                    if (response.StatusCode == HttpStatusCode.RequestTimeout ||
                        response.StatusCode == HttpStatusCode.GatewayTimeout ||
                        response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        Thread.Sleep(Configuration.RetryTimeout);
                        return Send(message);
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Reconnect();
                    }
                }
            }
            
            return true;
        }

        private void Reconnect()
        {
            try
            {
                if (StartReconnect())
                {
                    Configuration.Initialise();
                }
            }
            finally
            {
                EndReconnect();
            }
        }

        private bool StartReconnect()
        {
            return _flags.AddOrUpdate("reconnect", 1, (s, i) => i + 1) == 1;
        }

        private void EndReconnect()
        {
            _flags.TryRemove("reconnect", out var _);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Flush();
        }

        /// <summary>
        /// Adds a LogLine to the buffer.
        /// </summary>
        /// <param name="line">The line.</param>
        public void AddLine(LogLine line)
        {
            _buffer.Enqueue(line);
        }

        /// <summary>
        /// Attempts to flush the buffer.
        /// </summary>
        public void Flush()
        {
            if (StartFlush())
            {
                try
                {
                    var length = _buffer.Count;
                    var items = new List<LogLine>();

                    for (var i = 0; i < length; i++)
                    {
                        _buffer.TryDequeue(out var line);

                        if (line == null) break;

                        items.Add(line);
                    }

                    if (items.Any())
                    {
                        var message = new BufferMessage
                        {
                            LogLines = items
                        };

                        try
                        {
                            Send(JsonConvert.SerializeObject(message));
                        }
                        catch (Exception)
                        {
                            // Do nothing
                        }
                    }
                }
                finally
                {
                    EndFlush();
                }
            }
        }

        private bool StartFlush()
        {
            return _flags.AddOrUpdate("flushing", 1, (s, i) => i + 1) == 1;
        }

        private void EndFlush()
        {
            _flags.TryRemove("flushing", out var _);
        }

        private void InternalLogger(string message)
        {
            if (Configuration.LogInternalsToConsole)
                Console.WriteLine(message);
        }
    }
}

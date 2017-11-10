using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Newtonsoft.Json;

namespace RedBear.LogDNA
{
    /// <summary>
    /// Manages a local buffer of log lines and the transmission of these in chunks to the LogDNA servers.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class LogLineBuffer
    {
        private readonly ApiClient _client;

        public LogLineBuffer(ApiClient client)
        {
            _client = client;

            _buffer = new List<LogLine>();
            var timer = new Timer(_client.Configuration.FlushInterval);
            timer.Elapsed += _timer_Elapsed;
            timer.Start();
        }

        private readonly List<LogLine> _buffer;
        private bool _flushing;
        private readonly List<LogLine> _sending = new List<LogLine>();
        private static readonly object LogLock = new object();
        private static readonly object FlushLock = new object();

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
        public LogLine[] LogLines => _sending.ToArray();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LogLineBuffer"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        public bool Running { get; set; }

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
            Trace.WriteLine("Adding a new line..");
            lock (LogLock)
            {
                if (_buffer.Count + 1 > _client.Configuration.BufferLimit)
                {
                    Trace.WriteLine("Buffer reaching limit: remove earliest item..");
                    _buffer.RemoveAt(0);
                }

                _buffer.Add(line);

                if (_buffer.Count >= _client.Configuration.FlushLimit)
                {
                    Trace.WriteLine("Buffer has reached flush limit.");
                    Flush();
                }
            }
        }

        /// <summary>
        /// Attempts to flush the buffer.
        /// </summary>
        public void Flush()
        {
            Trace.WriteLine("Flushing..");

            if (_client.Active && !_flushing && Running && _buffer.Count > 0)
            {
                lock (FlushLock)
                {
                    _flushing = true;
                    _sending.AddRange(_buffer);
                    _buffer.Clear();

                    try
                    {
                        var result = _client.Send(JsonConvert.SerializeObject(this));
                        if (result) _sending.Clear();
                    }
                    catch (Exception)
                    {
                        // Do nothing
                    }

                    _flushing = false;
                }
            }
        }
    }
}

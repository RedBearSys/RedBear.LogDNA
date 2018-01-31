namespace RedBear.LogDNA
{
    public interface IApiClient
    {
        /// <summary>
        /// Whether the client is connectead to LogDNA.
        /// </summary>
        /// <returns></returns>
        bool Active { get; set; }

        /// <summary>
        /// Connects to the LogDNA servers using the specified configuration.
        /// </summary>
        /// <returns></returns>
        void Connect();

        /// <summary>
        /// Disconnects the client from the LogDNA servers.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the specified message directly to the websocket.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was transmitted successfully.</returns>
        bool Send(string message);

        /// <summary>
        /// Adds a LogLine to the buffer.
        /// </summary>
        /// <param name="line">The line.</param>
        void AddLine(LogLine line);

        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        void Flush();
    }
}
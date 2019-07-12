using RedBear.LogDNA;
using System;
using System.Threading;
// ReSharper disable once RedundantUsingDirective
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        private const int FlushTimeout = 30000;
        private const string IngestionKey = "PUT-KEY-HERE";

        //[Fact]
        public void DefaultLogsOk()
        {
            var config = new ConfigurationManager(IngestionKey) {Tags = new[] {"foo", "bar"}};
            var client = config.Initialise();

            client.Connect();

            client.AddLine(new LogLine("MyLog", "From Default Client"));

            Thread.Sleep(FlushTimeout);
            client.Disconnect();
        }

        //[Fact]
        public void HttpLogsOk()
        {
            var config = new ConfigurationManager(IngestionKey) { Tags = new[] { "foo", "bar" } };
            config.Initialise();

            var client = new HttpApiClient(config);

            client.Connect();

            client.AddLine(new LogLine("MyLog", "From HTTP Client"));

            Thread.Sleep(FlushTimeout);
            client.Disconnect();
        }

        //[Fact]
        public void SocketLogsOk()
        {
            var config = new ConfigurationManager(IngestionKey) { Tags = new[] { "foo", "bar" } };
            config.Initialise();

            var client = new SocketApiClient(config);

            client.Connect();

            client.AddLine(new LogLine("MyLog", "From WebSocket Client"));

            Thread.Sleep(FlushTimeout);
            client.Disconnect();
        }

        //[Fact]
        public void DefaultLogsLotsOk()
        {
            var config = new ConfigurationManager(IngestionKey) { Tags = new[] { "foo", "bar" } };
            var client = config.Initialise();

            client.Connect();

            for (var i = 0; i < 1000; i++)
            {
                client.AddLine(new LogLine("MyLog", $"From Default Client {i} {DateTime.UtcNow.ToShortTimeString()}"));
            }

            Thread.Sleep(FlushTimeout);
            client.Disconnect();
        }
    }
}

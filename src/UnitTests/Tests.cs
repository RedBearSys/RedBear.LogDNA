using RedBear.LogDNA;
using System.Threading;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        //[Fact]
        // ReSharper disable once InconsistentNaming
        public void AppearsInLogDNA()
        {
            var config = new Config("--KEY--");

            var client = new ApiClient(config);
            client.Connect();

            client.AddLine(new LogLine("MyLog", "My logged comment"));

            Thread.Sleep(2000);

            client.Disconnect();
        }
    }
}

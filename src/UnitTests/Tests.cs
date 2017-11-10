using RedBear.LogDNA;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Fact]
        // ReSharper disable once InconsistentNaming
        public async void AppearsInLogDNA()
        {
            var config = new Config("MyApp", "92a9b668db479870509621dbd354145d");

            var client = new ApiClient();
            await client.ConnectAsync(config);

            client.AddLine(new LogLine("MyLog", "My logged comment"));

            await Task.Delay(2000);
        }
    }
}

using RedBear.LogDNA;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        //[Fact]
        // ReSharper disable once InconsistentNaming
        public async void AppearsInLogDNA()
        {
            var config = new Config("MyApp", "--KEY--");

            var client = new ApiClient();
            await client.ConnectAsync(config);

            client.AddLine(new LogLine("MyLog", "My logged comment"));

            await Task.Delay(2000);
        }
    }
}

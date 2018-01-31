# RedBear.LogDNA
.NET Standard 2.0 client for the LogDNA service.

```
Install-Package RedBear.LogDNA
```

Allows log data to be sent to LogDNA using managed code.

```c#
private const int FlushTimeout = 30000;
private const string IngestionKey = "PUT-KEY-HERE";

[Fact]
public void DefaultLogsOk()
{
  var config = new ConfigurationManager(IngestionKey) {Tags = new[] {"foo", "bar"}};
  var client = config.Initialise();

  client.Connect();

  client.AddLine(new LogLine("MyLog", "From Default Client"));

  Thread.Sleep(FlushTimeout);
  client.Disconnect();
}
```

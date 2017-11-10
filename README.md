# RedBear.LogDNA
.NET Standard 2.0 client for the LogDNA service

Allows log data to be sent to LogDNA using managed code.

```c#
var config = new Config("MyApp", "my-logdna-key");
IApiClient client = new ApiClient();
            
await client.ConnectAsync(config);
client.AddLine(new LogLine("MyLog", "My logged comment"));
```

# RedBear.LogDNA
.NET Standard 2.0 client for the LogDNA service.

```
Install-Package RedBear.LogDNA
```

Allows log data to be sent to LogDNA using managed code.

```c#
var config = new Config("my-logdna-key");

// Implements IApiClient
var client = new ApiClient();
            
await client.ConnectAsync(config);
client.AddLine(new LogLine("MyLog", "My logged comment"));
```

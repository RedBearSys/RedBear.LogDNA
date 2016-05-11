# RedBear.LogDNA
.NET client for the LogDNA service

Allows log data to be sent to LogDNA using managed code.

```c#
var config = new Config("MyApp", "my-logdna-key");
await ApiClient.Connect(config);

ApiClient.AddLine(new LogLine("LogName", "My logged info", DateTime.Now());
```

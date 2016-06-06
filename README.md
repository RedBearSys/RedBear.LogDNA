# RedBear.LogDNA

**We are no longer using LogDNA ourselves, so we are no longer updating this repository and will not accept pull requests. Feel free to fork, however.**

.NET client for the LogDNA service

Allows log data to be sent to LogDNA using managed code.

```c#
var config = new Config("MyApp", "my-logdna-key");
await ApiClient.Connect(config);

ApiClient.AddLine(new LogLine("LogName", "My logged info", DateTime.Now());
```

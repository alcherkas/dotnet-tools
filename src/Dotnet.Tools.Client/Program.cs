using System.Collections.ObjectModel;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

await using var mcpClient = await McpClientFactory.CreateAsync(new()
{
    Id = "demo-server",
    Name = "Demo Server",
    TransportType = TransportTypes.Sse,
    Location = "http://localhost:5151/sse"
});

var tools = await mcpClient.ListToolsAsync();
var arguments = new Dictionary<string, object?>
{
    ["projectPath"] = ""
};
foreach (var tool in tools)
{
    Console.WriteLine("Calling " + tool.Name);

    var response = await mcpClient.CallToolAsync(tool.Name, arguments);
    Console.WriteLine(string.Join("", response.Content.Select(c => c.Text)));
}
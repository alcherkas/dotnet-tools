using System.ComponentModel;
using ModelContextProtocol.Server;
using System.Diagnostics;

namespace Dotnet.Tools.ServerSSE;

[McpServerToolType]
public class BuildTools
{
    [McpServerTool, Description("Lists all installed .NET SDK versions and their installation paths on the system")]
    public static async Task<List<string>> Version()
    {
        var output = await ExecuteDotnetCommand("--list-sdks");
        return output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    [McpServerTool, Description("Builds a .NET project or solution at the specified path")]
    public static async Task<string> Build([Description("Path to the .NET project or solution file")] string projectPath)
    {
        return await ExecuteDotnetCommand("build", projectPath);
    }
    
    [McpServerTool, Description("Runs tests for a .NET project or solution at the specified path")]
    public static async Task<string> Test([Description("Path to the .NET project or solution file")] string projectPath)
    {
        return await ExecuteDotnetCommand("test", projectPath);
    }

    private static async Task<string> ExecuteDotnetCommand(string command, params string[] arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{command} {string.Join(" ", arguments.Select(arg => $"\"{arg}\""))}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Command failed with exit code {process.ExitCode}. Error: {error}");
        }

        return output;
    }
}
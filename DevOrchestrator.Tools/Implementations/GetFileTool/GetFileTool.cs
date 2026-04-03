using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevOrchestrator.Tools.Implementations;

public class GetFileTool : Core.IMcpTool
{
    public string Name => "get_file";

    public Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("path", out var pathObj) || pathObj == null)
        {
            return Task.FromResult<object>(new { error = "Missing required argument: path" });
        }

        var path = pathObj.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            return Task.FromResult<object>(new { error = "Argument 'path' cannot be empty" });
        }

        if (!File.Exists(path))
        {
            return Task.FromResult<object>(new { error = $"File not found: {path}" });
        }

        try
        {
            var content = File.ReadAllText(path);
            return Task.FromResult<object>(new { path, content });
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new { error = ex.Message });
        }
    }
}

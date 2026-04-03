using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevOrchestrator.Tools.Implementations;

public class SearchCodeTool : Core.IMcpTool
{
    public string Name => "search_code";

    public Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("query", out var queryObj) || queryObj == null)
        {
            return Task.FromResult<object>(new { error = "Missing required argument: query" });
        }

        string query = queryObj.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<object>(new { error = "Argument 'query' cannot be empty" });

        string? rootPath = null;
        if (args.TryGetValue("rootPath", out var rootPathObj) && rootPathObj != null)
            rootPath = rootPathObj.ToString();

        rootPath ??= Directory.GetCurrentDirectory();

        if (!Directory.Exists(rootPath))
            return Task.FromResult<object>(new { error = $"Directory not found: {rootPath}" });

        var files = Directory.EnumerateFiles(rootPath, "*.cs", SearchOption.AllDirectories);
        var hits = new List<object>();

        foreach (var file in files)
        {
            try
            {
                var text = File.ReadAllText(file);
                if (text.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    hits.Add(new { file, snippet = GetSnippet(text, query) });
                }
            }
            catch
            {
                // ignore file read failures
            }
        }

        return Task.FromResult<object>(new
        {
            query,
            rootPath,
            hits
        });
    }

    private static string GetSnippet(string text, string query)
    {
        var idx = text.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (idx < 0 || text.Length < idx)
            return string.Empty;

        const int radius = 120;
        var start = Math.Max(0, idx - radius);
        var end = Math.Min(text.Length, idx + query.Length + radius);

        return text.Substring(start, end - start).Replace("\r\n", " ").Replace("\n", " ");
    }
}

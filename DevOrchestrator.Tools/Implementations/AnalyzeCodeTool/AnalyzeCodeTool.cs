using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Services;

namespace DevOrchestrator.Tools.Implementations;

public class AnalyzeCodeTool : Core.IMcpTool
{
    private readonly IAiService _aiService;

    public string Name => "analyze_code";

    public AnalyzeCodeTool(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        if (args == null || !args.TryGetValue("code", out var codeObj) || codeObj == null)
        {
            return new { error = "Missing required argument: code" };
        }

        var code = codeObj.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return new { error = "Argument 'code' cannot be empty" };
        }

        var analysis = await _aiService.AnalyzeCodeAsync(code);
        return new { analysis };
    }
}

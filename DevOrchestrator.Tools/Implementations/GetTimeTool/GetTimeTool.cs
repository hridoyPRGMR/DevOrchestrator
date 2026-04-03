using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Tools.Implementations;

public class GetTimeTool : Core.IMcpTool
{
    public string Name => "get_time";

    public Task<object> ExecuteAsync(Dictionary<string, object> args)
    {
        var now = DateTime.UtcNow;
        return Task.FromResult<object>(new
        {
            now = now,
            timezone = "utc"
        });
    }
}

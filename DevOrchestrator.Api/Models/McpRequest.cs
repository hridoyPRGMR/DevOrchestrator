using System.Collections.Generic;

namespace DevOrchestrator.Api.Models;

public class McpRequest
{
    public string? Tool { get; set; }
    public Dictionary<string, object>? Arguments { get; set; }
}

using System.Collections.Generic;

namespace DevOrchestrator.Api.Models;

public class AgentQueryRequest
{
    public string? Query { get; set; }
    public AgentContextDto? Context { get; set; }
    public string? SessionId { get; set; }
}

public class AgentContinueRequest
{
    public string? Query { get; set; }
    public string? SessionId { get; set; }
    public AgentContextDto? AdditionalContext { get; set; }
}

public class AgentContextDto
{
    public Guid? RepositoryId { get; set; }
    public List<string>? FilePaths { get; set; }
    public Dictionary<string, object>? CustomData { get; set; }
}

public class AgentQueryResponse
{
    public string? Content { get; set; }
    public List<ToolResultDto>? ToolResults { get; set; }
    public bool Success { get; set; }
    public string? SessionId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ToolResultDto
{
    public string? ToolName { get; set; }
    public object? Result { get; set; }
    public bool Success { get; set; }
    public long ExecutionTimeMs { get; set; }
    public Dictionary<string, object>? Arguments { get; set; }
}
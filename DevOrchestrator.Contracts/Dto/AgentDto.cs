using System.Collections.Generic;

namespace DevOrchestrator.Contracts.Dto;

// Agent API DTOs
public class AgentQueryRequestDto
{
    public string? Query { get; set; }
    public AgentContextDto? Context { get; set; }
    public string? SessionId { get; set; }
}

public class AgentContinueRequestDto
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

public class AgentQueryResponseDto
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

// MCP Tool DTOs
public class McpRequestDto
{
    public string? Tool { get; set; }
    public Dictionary<string, object>? Arguments { get; set; }
}

public class McpResponseDto
{
    public object? Result { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
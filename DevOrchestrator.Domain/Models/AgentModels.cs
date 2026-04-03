using System.Collections.Generic;

namespace DevOrchestrator.Domain.Models;

public record AgentResponse(
    string Content,
    List<ToolExecutionResult> ToolResults,
    bool Success,
    string? SessionId = null,
    Dictionary<string, object>? Metadata = null
);

public record ToolExecutionResult(
    string ToolName,
    object Result,
    bool Success,
    long ExecutionTimeMs,
    Dictionary<string, object>? Arguments = null
);

public record AgentContext(
    Guid? RepositoryId = null,
    List<string>? FilePaths = null,
    Dictionary<string, object>? CustomData = null
);
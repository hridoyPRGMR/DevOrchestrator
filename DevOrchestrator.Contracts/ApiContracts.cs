using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Contracts;

/// <summary>
/// Contract for agent operations
/// </summary>
public interface IAgentApi
{
    Task<Dto.AgentQueryResponseDto> ExecuteQueryAsync(Dto.AgentQueryRequestDto request);
    Task<Dto.AgentQueryResponseDto> ContinueConversationAsync(Dto.AgentContinueRequestDto request);
}

/// <summary>
/// Contract for MCP tool operations
/// </summary>
public interface IMcpApi
{
    Task<Dto.McpResponseDto> ExecuteToolAsync(Dto.McpRequestDto request);
}

/// <summary>
/// Contract for repository operations
/// </summary>
public interface IRepositoryApi
{
    Task<IEnumerable<Dto.RepositoryDto>> GetRepositoriesAsync();
    Task<Dto.RepositoryDto?> GetRepositoryAsync(Guid repositoryId);
    Task<Dto.RepositoryFilesResponseDto> GetRepositoryFilesAsync(Guid repositoryId, string? path = null);
}

/// <summary>
/// Contract for file operations
/// </summary>
public interface IFileApi
{
    Task<Dto.FileContentDto?> GetFileContentAsync(Guid repositoryId, string filePath);
    Task<Dto.FileAnalysisDto> AnalyzeFileAsync(Guid repositoryId, string filePath);
}
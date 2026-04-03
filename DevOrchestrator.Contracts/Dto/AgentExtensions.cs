using DevOrchestrator.Domain.Models;

namespace DevOrchestrator.Contracts.Dto;

public static class AgentExtensions
{
    public static AgentQueryRequestDto ToDto(this object request)
    {
        // This would be used if we had domain request models
        // For now, the DTOs are used directly
        throw new NotImplementedException();
    }

    public static AgentQueryResponseDto ToDto(this AgentResponse response)
    {
        return new AgentQueryResponseDto
        {
            Content = response.Content,
            ToolResults = response.ToolResults.Select(tr => new ToolResultDto
            {
                ToolName = tr.ToolName,
                Result = tr.Result,
                Success = tr.Success,
                ExecutionTimeMs = tr.ExecutionTimeMs,
                Arguments = tr.Arguments
            }).ToList(),
            Success = response.Success,
            SessionId = response.SessionId,
            Metadata = response.Metadata
        };
    }

    public static AgentContext ToDomain(this AgentContextDto context)
    {
        return new AgentContext(
            RepositoryId: context.RepositoryId,
            FilePaths: context.FilePaths,
            CustomData: context.CustomData
        );
    }
}
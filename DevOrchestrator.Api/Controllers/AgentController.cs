using DevOrchestrator.Api.Models;
using DevOrchestrator.Domain.Models;
using DevOrchestrator.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevOrchestrator.Api.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController : ControllerBase
{
    private readonly IAgentEngine _agentEngine;

    public AgentController(IAgentEngine agentEngine)
    {
        _agentEngine = agentEngine;
    }

    /// <summary>
    /// Execute a query using the AI agent with multi-tool orchestration
    /// </summary>
    [HttpPost("query")]
    public async Task<IActionResult> ExecuteQuery([FromBody] AgentQueryRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { error = "Query is required" });
        }

        var context = request.Context != null ? new AgentContext(
            RepositoryId: request.Context.RepositoryId,
            FilePaths: request.Context.FilePaths,
            CustomData: request.Context.CustomData
        ) : null;

        var response = await _agentEngine.ExecuteQueryAsync(request.Query, context, request.SessionId);

        return Ok(new AgentQueryResponse
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
        });
    }

    /// <summary>
    /// Continue a conversation with the agent
    /// </summary>
    [HttpPost("continue")]
    public async Task<IActionResult> ContinueConversation([FromBody] AgentContinueRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query) || string.IsNullOrWhiteSpace(request.SessionId))
        {
            return BadRequest(new { error = "Query and SessionId are required" });
        }

        var context = request.AdditionalContext != null ? new AgentContext(
            RepositoryId: request.AdditionalContext.RepositoryId,
            FilePaths: request.AdditionalContext.FilePaths,
            CustomData: request.AdditionalContext.CustomData
        ) : null;

        var response = await _agentEngine.ContinueConversationAsync(request.Query, request.SessionId, context);

        return Ok(new AgentQueryResponse
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
        });
    }
}
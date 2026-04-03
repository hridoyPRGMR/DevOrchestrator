using DevOrchestrator.Contracts.Dto;
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
    public async Task<IActionResult> ExecuteQuery([FromBody] AgentQueryRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { error = "Query is required" });
        }

        var context = request.Context != null ? request.Context.ToDomain() : null;

        var response = await _agentEngine.ExecuteQueryAsync(request.Query, context, request.SessionId);

        return Ok(response.ToDto());
    }

    /// <summary>
    /// Continue a conversation with the agent
    /// </summary>
    [HttpPost("continue")]
    public async Task<IActionResult> ContinueConversation([FromBody] AgentContinueRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query) || string.IsNullOrWhiteSpace(request.SessionId))
        {
            return BadRequest(new { error = "Query and SessionId are required" });
        }

        var context = request.AdditionalContext != null ? request.AdditionalContext.ToDomain() : null;

        var response = await _agentEngine.ContinueConversationAsync(request.Query, request.SessionId, context);

        return Ok(response.ToDto());
    }
}
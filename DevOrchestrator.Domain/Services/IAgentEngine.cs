using System.Collections.Generic;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Models;

namespace DevOrchestrator.Domain.Services;

public interface IAgentEngine
{
    /// <summary>
    /// Executes a user query by orchestrating multiple tools and AI calls.
    /// </summary>
    /// <param name="query">The user's natural language query</param>
    /// <param name="context">Optional context information (repository, files, etc.)</param>
    /// <param name="sessionId">Optional session ID for conversation continuity</param>
    /// <returns>Agent response with tool execution results and AI-generated content</returns>
    Task<AgentResponse> ExecuteQueryAsync(string query, AgentContext? context = null, string? sessionId = null);

    /// <summary>
    /// Continues a conversation with additional context from previous interactions.
    /// </summary>
    /// <param name="followUpQuery">The follow-up query</param>
    /// <param name="sessionId">Session ID to maintain conversation context</param>
    /// <param name="additionalContext">Additional context from the conversation</param>
    /// <returns>Agent response with continued conversation</returns>
    Task<AgentResponse> ContinueConversationAsync(string followUpQuery, string sessionId, AgentContext? additionalContext = null);
}
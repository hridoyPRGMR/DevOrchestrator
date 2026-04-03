using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DevOrchestrator.Domain.Models;
using DevOrchestrator.Domain.Services;
using DevOrchestrator.Tools.Core;
using Microsoft.Extensions.Logging;

namespace DevOrchestrator.Infrastructure.Services;

public class AgentEngine : IAgentEngine
{
    private readonly IAiService _aiService;
    private readonly IMcpToolRegistry _toolRegistry;
    private readonly ILogger<AgentEngine> _logger;
    private readonly ICacheService _cacheService;

    public AgentEngine(
        IAiService aiService,
        IMcpToolRegistry toolRegistry,
        ILogger<AgentEngine> logger,
        ICacheService cacheService)
    {
        _aiService = aiService;
        _toolRegistry = toolRegistry;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<AgentResponse> ExecuteQueryAsync(string query, AgentContext? context = null, string? sessionId = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var toolResults = new List<ToolExecutionResult>();
        var sessionIdToUse = sessionId ?? Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Executing agent query: {Query}", query);

            // Step 1: Analyze the query and determine which tools to use
            var toolPlan = await PlanToolExecutionAsync(query, context);

            // Step 2: Execute the planned tools
            foreach (var toolCall in toolPlan.ToolCalls)
            {
                var result = await ExecuteToolAsync(toolCall);
                toolResults.Add(result);
            }

            // Step 3: Generate a comprehensive response using AI
            var response = await GenerateResponseAsync(query, toolResults, context);

            stopwatch.Stop();

            return new AgentResponse(
                Content: response.Content,
                ToolResults: toolResults,
                Success: true,
                SessionId: sessionIdToUse,
                Metadata: new Dictionary<string, object>
                {
                    ["executionTimeMs"] = stopwatch.ElapsedMilliseconds,
                    ["toolsUsed"] = toolResults.Count,
                    ["cacheHit"] = response.CacheHit
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing agent query: {Query}", query);
            stopwatch.Stop();

            return new AgentResponse(
                Content: $"I encountered an error while processing your request: {ex.Message}",
                ToolResults: toolResults,
                Success: false,
                SessionId: sessionIdToUse,
                Metadata: new Dictionary<string, object>
                {
                    ["executionTimeMs"] = stopwatch.ElapsedMilliseconds,
                    ["error"] = ex.Message
                }
            );
        }
    }

    public async Task<AgentResponse> ContinueConversationAsync(string followUpQuery, string sessionId, AgentContext? additionalContext = null)
    {
        // For now, treat as a new query. In a full implementation, this would maintain conversation state
        return await ExecuteQueryAsync(followUpQuery, additionalContext, sessionId);
    }

    private async Task<ToolExecutionPlan> PlanToolExecutionAsync(string query, AgentContext? context)
    {
        var availableTools = _toolRegistry.GetToolNames().ToList();

        // Create a prompt for the AI to plan tool usage
        var planningPrompt = BuildPlanningPrompt(query, availableTools, context);

        var aiResponse = await _aiService.GenerateAsync(planningPrompt);

        // Parse the AI response to determine which tools to call
        return ParseToolPlan(aiResponse.Content, availableTools);
    }

    private string BuildPlanningPrompt(string query, List<string> availableTools, AgentContext? context)
    {
        var prompt = new System.Text.StringBuilder();
        prompt.AppendLine("You are an AI assistant that can use various tools to help users with development tasks.");
        prompt.AppendLine();
        prompt.AppendLine("Available tools:");
        foreach (var tool in availableTools)
        {
            prompt.AppendLine($"- {tool}");
        }
        prompt.AppendLine();
        prompt.AppendLine("User query: " + query);
        prompt.AppendLine();

        if (context != null)
        {
            prompt.AppendLine("Context:");
            if (context.RepositoryId.HasValue)
                prompt.AppendLine($"- Repository ID: {context.RepositoryId}");
            if (context.FilePaths != null && context.FilePaths.Any())
                prompt.AppendLine($"- Relevant files: {string.Join(", ", context.FilePaths)}");
            prompt.AppendLine();
        }

        prompt.AppendLine("Please analyze the query and determine which tools should be called to provide the best answer.");
        prompt.AppendLine("Respond with a JSON object containing a 'toolCalls' array. Each tool call should have 'toolName' and 'arguments' fields.");
        prompt.AppendLine("Example: {\"toolCalls\": [{\"toolName\": \"search_code\", \"arguments\": {\"query\": \"function\"}}]}");

        return prompt.ToString();
    }

    private ToolExecutionPlan ParseToolPlan(string aiResponse, List<string> availableTools)
    {
        try
        {
            // Try to parse as JSON
            var plan = JsonSerializer.Deserialize<ToolExecutionPlan>(aiResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (plan?.ToolCalls != null)
            {
                // Validate that the tools exist
                plan.ToolCalls = plan.ToolCalls
                    .Where(call => availableTools.Contains(call.ToolName, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                return plan;
            }
        }
        catch (JsonException)
        {
            _logger.LogWarning("Failed to parse AI tool planning response as JSON: {Response}", aiResponse);
        }

        // Fallback: try to extract tool names from the response text
        return ExtractToolsFromText(aiResponse, availableTools);
    }

    private ToolExecutionPlan ExtractToolsFromText(string response, List<string> availableTools)
    {
        var toolCalls = new List<ToolCall>();

        foreach (var toolName in availableTools)
        {
            if (response.Contains(toolName, StringComparison.OrdinalIgnoreCase))
            {
                toolCalls.Add(new ToolCall
                {
                    ToolName = toolName,
                    Arguments = new Dictionary<string, object>() // Empty args for now
                });
            }
        }

        return new ToolExecutionPlan { ToolCalls = toolCalls };
    }

    private async Task<ToolExecutionResult> ExecuteToolAsync(ToolCall toolCall)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var tool = _toolRegistry.Get(toolCall.ToolName);
            if (tool == null)
            {
                return new ToolExecutionResult(
                    ToolName: toolCall.ToolName,
                    Result: new { error = "Tool not found" },
                    Success: false,
                    ExecutionTimeMs: 0,
                    Arguments: toolCall.Arguments
                );
            }

            var result = await tool.ExecuteAsync(toolCall.Arguments ?? new Dictionary<string, object>());
            stopwatch.Stop();

            return new ToolExecutionResult(
                ToolName: toolCall.ToolName,
                Result: result,
                Success: true,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                Arguments: toolCall.Arguments
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error executing tool {ToolName}", toolCall.ToolName);

            return new ToolExecutionResult(
                ToolName: toolCall.ToolName,
                Result: new { error = ex.Message },
                Success: false,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                Arguments: toolCall.Arguments
            );
        }
    }

    private async Task<AiResponse> GenerateResponseAsync(string query, List<ToolExecutionResult> toolResults, AgentContext? context)
    {
        var responsePrompt = BuildResponsePrompt(query, toolResults, context);
        return await _aiService.GenerateAsync(responsePrompt);
    }

    private string BuildResponsePrompt(string query, List<ToolExecutionResult> toolResults, AgentContext? context)
    {
        var prompt = new System.Text.StringBuilder();
        prompt.AppendLine("Based on the following tool execution results, provide a comprehensive answer to the user's query.");
        prompt.AppendLine();
        prompt.AppendLine("User query: " + query);
        prompt.AppendLine();

        if (context != null && context.FilePaths != null && context.FilePaths.Any())
        {
            prompt.AppendLine("Context files: " + string.Join(", ", context.FilePaths));
            prompt.AppendLine();
        }

        prompt.AppendLine("Tool execution results:");
        foreach (var result in toolResults)
        {
            prompt.AppendLine($"- {result.ToolName}: {JsonSerializer.Serialize(result.Result)}");
        }
        prompt.AppendLine();

        prompt.AppendLine("Please provide a helpful, detailed response that synthesizes the information from the tools.");

        return prompt.ToString();
    }

    private record ToolExecutionPlan
    {
        public List<ToolCall> ToolCalls { get; set; } = new();
    }

    private record ToolCall
    {
        public string ToolName { get; set; } = string.Empty;
        public Dictionary<string, object> Arguments { get; set; } = new();
    }
}
using DevOrchestrator.Contracts.Dto;
using DevOrchestrator.Tools.Core;
using Microsoft.AspNetCore.Mvc;

namespace DevOrchestrator.Api.Controllers;

[ApiController]
[Route("mcp")]
public class McpController : ControllerBase
{
    private readonly IMcpToolRegistry _toolRegistry;

    public McpController(IMcpToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] McpRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Tool))
        {
            return BadRequest(new McpResponseDto
            {
                Success = false,
                Error = "Tool name is required"
            });
        }

        var tool = _toolRegistry.Get(request.Tool);
        if (tool == null)
        {
            return NotFound(new McpResponseDto
            {
                Success = false,
                Error = $"Tool '{request.Tool}' not found",
                Result = new { availableTools = _toolRegistry.GetToolNames() }
            });
        }

        try
        {
            var result = await tool.ExecuteAsync(request.Arguments ?? new Dictionary<string, object>());
            return Ok(new McpResponseDto
            {
                Result = result,
                Success = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new McpResponseDto
            {
                Success = false,
                Error = $"Tool execution failed: {ex.Message}"
            });
        }
    }
}

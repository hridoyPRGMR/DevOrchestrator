using DevOrchestrator.Api.Models;
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
    public async Task<IActionResult> Post([FromBody] McpRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Tool))
        {
            return BadRequest(new { error = "Tool name is required" });
        }

        var tool = _toolRegistry.Get(request.Tool);
        if (tool == null)
        {
            return NotFound(new { error = $"Tool '{request.Tool}' not found", availableTools = _toolRegistry.GetToolNames() });
        }

        var result = await tool.ExecuteAsync(request.Arguments ?? new Dictionary<string, object>());
        return Ok(result);
    }
}

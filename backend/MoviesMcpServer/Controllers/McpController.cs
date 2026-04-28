using Microsoft.AspNetCore.Mvc;
using MoviesMcpServer.Models;
using MoviesMcpServer.Services;
using System.Text.Json;

namespace MoviesMcpServer.Controllers;

[ApiController]
[Route("[controller]")]
public class McpController : ControllerBase
{
    private readonly McpToolsService _toolsService;

    public McpController(McpToolsService toolsService)
    {
        _toolsService = toolsService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] JsonRpcRequest request)
    {
        try
        {
            object? result = request.Method switch
            {
                "initialize" => HandleInitialize(),
                "tools/list" => HandleToolsList(),
                "tools/call" => await HandleToolCall(request.Params),
                _ => null
            };

            if (result is null)
            {
                return Ok(new JsonRpcResponse
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32601,
                        Message = $"Method not found: {request.Method}"
                    }
                });
            }

            return Ok(new JsonRpcResponse
            {
                Id = request.Id,
                Result = result
            });
        }
        catch (Exception ex)
        {
            return Ok(new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError
                {
                    Code = -32603,
                    Message = ex.Message
                }
            });
        }
    }

    private McpInitializeResult HandleInitialize()
    {
        return new McpInitializeResult();
    }

    private McpToolsListResult HandleToolsList()
    {
        return new McpToolsListResult
        {
            Tools = _toolsService.GetAvailableTools()
        };
    }

    private async Task<McpToolCallResult> HandleToolCall(JsonElement? parameters)
    {
        if (parameters is null)
            throw new ArgumentException("Tool call requires parameters");

        var paramsObj = parameters.Value;
        var toolName = paramsObj.GetProperty("name").GetString() ?? "";
        var arguments = paramsObj.GetProperty("arguments");

        var resultText = await _toolsService.ExecuteToolAsync(toolName, arguments);

        return new McpToolCallResult
        {
            Content = new List<McpContent>
            {
                new McpContent { Type = "text", Text = resultText }
            }
        };
    }

    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "movies-mcp-server" });
}
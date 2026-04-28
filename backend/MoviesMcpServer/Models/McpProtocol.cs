using System.Text.Json.Serialization;

namespace MoviesMcpServer.Models;


public class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public System.Text.Json.JsonElement? Params { get; set; }
}

public class JsonRpcResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    public JsonRpcError? Error { get; set; }
}

public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}


public class McpServerInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "movies-mcp-server";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";
}

public class McpInitializeResult
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("serverInfo")]
    public McpServerInfo ServerInfo { get; set; } = new();

    [JsonPropertyName("capabilities")]
    public McpCapabilities Capabilities { get; set; } = new();
}

public class McpCapabilities
{
    [JsonPropertyName("tools")]
    public Dictionary<string, object> Tools { get; set; } = new();
}

public class McpTool
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("inputSchema")]
    public object InputSchema { get; set; } = new { };
}

public class McpToolsListResult
{
    [JsonPropertyName("tools")]
    public List<McpTool> Tools { get; set; } = new();
}

public class McpToolCallResult
{
    [JsonPropertyName("content")]
    public List<McpContent> Content { get; set; } = new();
}

public class McpContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
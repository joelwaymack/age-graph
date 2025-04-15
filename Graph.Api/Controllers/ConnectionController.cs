using Graph.Api.DataAccess;
using Graph.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Graph.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("connections")]
public class ConnectionController : ControllerBase
{
    private readonly GraphDatabase _graphDatabase;
    private readonly ILogger<VertexController> _logger;

    public ConnectionController(GraphDatabase graphDatabase, ILogger<VertexController> logger)
    {
        _graphDatabase = graphDatabase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IList<Connection>> GetAllConnections()
    {
        return await _graphDatabase.GetConnectionsAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreateConnection([FromBody] Connection connection)
    {
        if (connection == null)
        {
            return BadRequest("Connection cannot be null.");
        }

        if (connection.FromVertex == null || connection.Edge == null || connection.ToVertex == null)
        {
            return BadRequest("Connection must have FromVertex, Edge, and ToVertex properties set.");
        }

        var createdConnection = await _graphDatabase.CreateEdgeAsync(connection.FromVertex, connection.Edge, connection.ToVertex);
        return CreatedAtAction(nameof(GetAllConnections), new { }, createdConnection);
    }
}
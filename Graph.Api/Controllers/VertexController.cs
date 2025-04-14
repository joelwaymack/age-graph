using Graph.Api.DataAccess;
using Graph.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Age.Types;

namespace Graph.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("vertices")]
public class VertexController : ControllerBase
{
    private readonly GraphDatabase _graphDatabase;
    private readonly ILogger<VertexController> _logger;

    public VertexController(GraphDatabase graphDatabase, ILogger<VertexController> logger)
    {
        _graphDatabase = graphDatabase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IList<Vertex>> GetAllVertices([FromQuery] string label = null)
    {
        return await _graphDatabase.GetAllVerticesAsync(label);
    }

    [HttpGet("{label}")]
    public async Task<IList<Vertex>> GetVerticesByLabel(string label)
    {
        return await _graphDatabase.GetAllVerticesAsync(label);
    }

    [HttpGet("{label}/{id}")]
    public async Task<IList<Connection>> GetVertexEdges(string label, string id, [FromQuery] EdgeType? edgeType = null)
    {
        var connections = await _graphDatabase.GetVertexEdgesAsync(label, id, edgeType);
        return connections;
    }

    // [HttpPost("{label}")]
    // public async Task<IActionResult> CreateVertex(string label, [FromBody] JsonDocument vertex)
    // {
    //     if (string.IsNullOrWhiteSpace(label))
    //     {
    //         return BadRequest("Label cannot be null or empty.");
    //     }

    //     if (vertex == null)
    //     {
    //         return BadRequest("Vertex cannot be null.");
    //     }

    //     await _graphDatabase.CreateVertexAsync(label, vertex);
    //     return Created();
    // }
}
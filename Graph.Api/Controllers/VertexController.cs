using Graph.Api.DataAccess;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("labels")]
    public async Task<IList<string>> GetAllVertexLabels()
    {
        return await _graphDatabase.GetAllVertexLabelsAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreateVertex([FromBody] Vertex vertex)
    {
        if (vertex == null || string.IsNullOrEmpty(vertex.Label))
        {
            return BadRequest("Invalid vertex data.");
        }

        var createdVertex = await _graphDatabase.CreateVertexAsync(vertex);
        return CreatedAtAction(nameof(GetVerticesByLabel), new { label = createdVertex.Label }, createdVertex);
    }
}
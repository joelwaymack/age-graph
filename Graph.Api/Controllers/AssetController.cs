using Graph.Api.DataAccess;
using Graph.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Age.Types;

namespace Graph.Api.Controllers;

[ApiController]
[Route("assets")]
public class AssetController : ControllerBase
{
    private readonly GraphDatabase _graphDatabase;
    private readonly ILogger<AssetController> _logger;

    public AssetController(GraphDatabase graphDatabase, ILogger<AssetController> logger)
    {
        _graphDatabase = graphDatabase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IList<Vertex>> GetAllAssets()
    {
        return await _graphDatabase.GetAllVerticesAsync<Asset>();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsset([FromBody] Asset asset)
    {
        if (asset == null)
        {
            return BadRequest("Asset cannot be null.");
        }

        try
        {
            await _graphDatabase.CreateVertexAsync(asset);
            return CreatedAtAction(nameof(CreateAsset), new { id = asset.Id }, asset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{fromId}/{edgeType}/{toId}")]
    public async Task<IActionResult> CreateEdge(string fromId, EdgeType edgeType, string toId)
    {
        if (string.IsNullOrWhiteSpace(fromId) || string.IsNullOrWhiteSpace(toId))
        {
            return BadRequest("FromId and ToId cannot be null or empty.");
        }

        try
        {
            var edge = await _graphDatabase.CreateEdgeAsync<Asset>(fromId, edgeType, toId);
            return Created("", edge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating edge");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/edges")]
    public async Task<IActionResult> GetAssetEdges(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Id cannot be null or empty.");
        }

        try
        {
            var edges = await _graphDatabase.GetEdgesAsync<Asset>(id);
            return Ok(edges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset edges");
            return StatusCode(500, "Internal server error");
        }
    }
}
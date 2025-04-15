using Graph.Api.DataAccess;
using Graph.Api.Models;
using Microsoft.AspNetCore.Mvc;

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
}
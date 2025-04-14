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
}
using Graph.Api.Models;
using Npgsql;
using Age = Npgsql.Age.Types;

namespace Graph.Api.DataAccess;

public class GraphDatabase
{
    private readonly ILogger<GraphDatabase> _logger;
    private readonly NpgsqlDataSource _dataSource;
    private readonly PostgresConfig _config;

    public GraphDatabase(ILogger<GraphDatabase> logger, NpgsqlDataSource dataSource, PostgresConfig config)
    {
        _logger = logger;
        _dataSource = dataSource;
        _config = config;
    }

    public async Task<Vertex> CreateVertexAsync<T>(T vertex) where T : class
    {
        return await CreateVertexAsync(Vertex.FromTypedVertex(vertex));
    }

    public async Task<Vertex> CreateVertexAsync(Vertex vertex)
    {
        if (string.IsNullOrWhiteSpace(vertex.Label))
        {
            throw new ArgumentException("Label cannot be null or empty.", nameof(vertex.Label));
        }

        if (vertex == null)
        {
            throw new ArgumentNullException(nameof(vertex), "Vertex cannot be null.");
        }

        var query = ComposeQuery($"CREATE (v:{vertex.Label} {vertex.SerializeProperties()}) return v", "v agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        return Vertex.FromAgeVertex(reader.GetFieldValue<Age.Agtype>(0).GetVertex());
    }

    public async Task<IList<Vertex>> GetAllVerticesAsync<T>() where T : class
    {
        return await GetAllVerticesAsync(Vertex.GetLabel<T>());
    }

    public async Task<IList<Vertex>> GetAllVerticesAsync(string label)
    {
        var query = !string.IsNullOrWhiteSpace(label) ?
            ComposeQuery($"MATCH (v:{label}) return v", "v agtype") :
            ComposeQuery($"MATCH (v) return v", "v agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        var vertices = new List<Vertex>();
        while (await reader.ReadAsync())
        {
            var result = reader.GetFieldValue<Age.Agtype>(0);
            vertices.Add(Vertex.FromAgeVertex(result.GetVertex()));
        }

        return vertices;
    }

    public async Task<Connection> CreateEdgeAsync(Vertex fromVertex, Edge edge, Vertex toVertex)
    {
        var query = ComposeQuery(@$"MATCH (f:{fromVertex.Label} {fromVertex.SerializeProperties()}), (t:{toVertex.Label} {toVertex.SerializeProperties()}) 
            CREATE (f)-[e:{edge.GetEdgeLabel()} {edge.SerializeProperties()}]->(t) return f, e, t",
            "f agtype, e agtype, t agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        return new Connection
        {
            FromVertex = Vertex.FromAgeVertex(reader.GetFieldValue<Age.Agtype>(0).GetVertex()),
            Edge = Edge.FromAgeEdge(reader.GetFieldValue<Age.Agtype>(1).GetEdge()),
            ToVertex = Vertex.FromAgeVertex(reader.GetFieldValue<Age.Agtype>(2).GetVertex())
        };
    }

    public async Task<IList<Connection>> GetConnectionsAsync()
    {
        var query = ComposeQuery($"MATCH (f)-[e]->(t) return f, e, t", "f agtype, e agtype, t agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        var connections = new List<Connection>();
        while (await reader.ReadAsync())
        {

            var from = reader.GetFieldValue<Age.Agtype>(0).GetVertex();
            var e = reader.GetFieldValue<Age.Agtype>(1).GetEdge();
            var to = reader.GetFieldValue<Age.Agtype>(2).GetVertex();

            connections.Add(new Connection
            {
                FromVertex = Vertex.FromAgeVertex(from),
                Edge = Edge.FromAgeEdge(e),
                ToVertex = Vertex.FromAgeVertex(to)
            });
        }

        return connections;
    }

    public async Task<IList<string>> GetAllVertexLabelsAsync()
    {
        var query = ComposeQuery("MATCH (n) RETURN DISTINCT labels(n) AS vertex_labels", "vertext_labels agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        var result = reader.GetFieldValue<Age.Agtype>(0).GetList();
        var labels = result.Where(l => l is string).Select(l => l.ToString()).ToList();

        return labels;
    }

    private string ComposeQuery(string cypher, string returnSet)
    {
        return @$"SET search_path = ag_catalog, ""$user"", public;
            SELECT * FROM ag_catalog.cypher('{_config.GraphName}', $$
            {cypher}
            $$) as ({returnSet});";
    }


}
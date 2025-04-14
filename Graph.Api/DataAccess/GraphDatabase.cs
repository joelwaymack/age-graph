using System.Text.Json;
using Graph.Api.Models;
using Npgsql;
using Npgsql.Age.Types;

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
        var query = ComposeQuery($"CREATE (v:{GetLabel<T>()} {PropertySerializer.SerializeProperties(vertex)}) return v", "v agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        return reader.GetFieldValue<Agtype>(0).GetVertex();
    }

    public async Task<IList<Vertex>> GetAllVerticesAsync<T>() where T : class
    {
        var query = ComposeQuery($"MATCH (v:{GetLabel<T>()}) return v", "v agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        var vertices = new List<Vertex>();
        while (await reader.ReadAsync())
        {
            var result = reader.GetFieldValue<Agtype>(0);
            vertices.Add(result.GetVertex());
        }

        return vertices;
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
            var result = reader.GetFieldValue<Agtype>(0);
            vertices.Add(result.GetVertex());
        }

        return vertices;
    }

    public async Task<Vertex> CreateVertexAsync(string label, JsonDocument vertex)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Label cannot be null or empty.", nameof(label));
        }

        if (vertex == null)
        {
            throw new ArgumentNullException(nameof(vertex), "Vertex cannot be null.");
        }

        var query = ComposeQuery($"CREATE (v:{label} {PropertySerializer.SerializeJsonDocument(vertex)}) return v", "v agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        return reader.GetFieldValue<Agtype>(0).GetVertex();
    }

    public async Task<Edge> CreateEdgeAsync<T>(string fromId, EdgeType edgeType, string toId) where T : class
    {
        var query = ComposeQuery($"MATCH (a:{GetLabel<T>()} {{id: '{fromId}'}}), (b:{GetLabel<T>()} {{id: '{toId}'}}) CREATE (a)-[e:{GetEdgeType(edgeType)}]->(b) return a, e, b",
            "a agtype, e agtype, b agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        // This is returning 1 row with 3 columns not rows
        var vertices = new List<Agtype>();
        while (await reader.ReadAsync())
        {
            var result = reader.GetFieldValue<Agtype>(0);
            vertices.Add(result);
        }

        return vertices.First(x => x.IsEdge).GetEdge();
    }

    internal async Task<IList<Agtype>> GetEdgesAsync<T>(string id) where T : class
    {
        var query = ComposeQuery($"MATCH (a:{GetLabel<T>()} {{id: '{id}'}} )-[e]->(b) WHERE a.id = '{id}' return a, e, b", "a agtype, e agtype, b agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        // This is returning 3 column rows
        var data = new List<Agtype>();
        while (await reader.ReadAsync())
        {
            var result = reader.GetFieldValue<Agtype>(0);
            data.Add(result);
        }

        return data;
    }

    public async Task<IList<Connection>> GetVertexEdgesAsync(string label, string id, EdgeType? edgeType)
    {
        var query = ComposeQuery($"MATCH (a:{label} {{id: '{id}'}})-[e1]-(b1) return a, e1, b1", "a agtype, e agtype, b agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        var connections = new List<Connection>();
        while (await reader.ReadAsync())
        {

            var v1 = reader.GetFieldValue<Agtype>(0).GetVertex();
            var edge = reader.GetFieldValue<Agtype>(1).GetEdge();
            var v2 = reader.GetFieldValue<Agtype>(2).GetVertex();

            connections.Add(new Connection
            {
                FromVertex = edge.StartId == v1.Id ? GraphItem.FromVertex(v1) : GraphItem.FromVertex(v2),
                Edge = GraphItem.FromEdge(edge),
                ToVertex = edge.StartId == v1.Id ? GraphItem.FromVertex(v2) : GraphItem.FromVertex(v1)
            });
        }

        return connections;
    }

    public async Task<IList<Connection>> GetConnectionsAsync()
    {
        var query = ComposeQuery($"MATCH (a)-[e]-(b) return a, e, b", "a agtype, e agtype, b agtype");

        await using var command = _dataSource.CreateCommand(query);
        await using var reader = await command.ExecuteReaderAsync();

        var connections = new List<Connection>();
        while (await reader.ReadAsync())
        {

            var v1 = reader.GetFieldValue<Agtype>(0).GetVertex();
            var edge = reader.GetFieldValue<Agtype>(1).GetEdge();
            var v2 = reader.GetFieldValue<Agtype>(2).GetVertex();

            connections.Add(new Connection
            {
                FromVertex = edge.StartId == v1.Id ? GraphItem.FromVertex(v1) : GraphItem.FromVertex(v2),
                Edge = GraphItem.FromEdge(edge),
                ToVertex = edge.StartId == v1.Id ? GraphItem.FromVertex(v2) : GraphItem.FromVertex(v1)
            });
        }

        return connections;
    }

    private string ComposeQuery(string cypher, string returnSet)
    {
        return @$"SET search_path = ag_catalog, ""$user"", public;
            SELECT * FROM ag_catalog.cypher('{_config.GraphName}', $$
            {cypher}
            $$) as ({returnSet});";
    }

    private string GetLabel<T>() where T : class
    {
        var type = typeof(T);
        var label = type.Name;
        return label;
    }

    private string GetEdgeType(EdgeType edgeType)
    {
        return edgeType.ToString();
    }
}
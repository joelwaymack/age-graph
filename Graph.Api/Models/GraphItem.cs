using Npgsql.Age.Types;

namespace Graph.Api.Models;

public class GraphItem
{
    public string Label { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public static GraphItem FromVertex(Vertex vertex)
    {
        return new GraphItem
        {
            Label = vertex.Label,
            Properties = vertex.Properties.ToDictionary(p => p.Key, p => p.Value)
        };
    }

    public static GraphItem FromEdge(Edge edge)
    {
        return new GraphItem
        {
            Label = edge.Label,
            Properties = edge.Properties.ToDictionary(p => p.Key, p => p.Value)
        };
    }
}
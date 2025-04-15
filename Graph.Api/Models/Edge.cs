using Age = Npgsql.Age.Types;

namespace Graph.Api.Models;

public class Edge
{
    public EdgeType Type { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public string GetEdgeLabel()
    {
        return Type.ToString();
    }

    public static Edge FromAgeEdge(Age.Edge edge)
    {
        return new Edge
        {
            Type = (EdgeType)Enum.Parse(typeof(EdgeType), edge.Label),
            Properties = edge.Properties.ToDictionary(p => p.Key, p => p.Value)
        };
    }
}
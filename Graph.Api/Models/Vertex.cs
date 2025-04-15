using System.Text.Json;
using Age = Npgsql.Age.Types;

public class Vertex
{
    public string Label { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public static Vertex FromAgeVertex(Age.Vertex vertex)
    {
        return new Vertex
        {
            Label = vertex.Label,
            Properties = vertex.Properties.ToDictionary(p => p.Key, p => p.Value)
        };
    }

    public static Vertex FromTypedVertex<T>(T vertex) where T : class
    {
        var label = GetLabel<T>();
        var properties = vertex.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(vertex));

        return new Vertex
        {
            Label = label,
            Properties = properties
        };
    }

    public static string GetLabel<T>() where T : class
    {
        var type = typeof(T);
        var label = type.Name;
        return label;
    }
}
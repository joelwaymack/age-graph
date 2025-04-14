using Npgsql.Age.Types;

namespace Graph.Api.Models;

public class Asset : VertexBase
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
}
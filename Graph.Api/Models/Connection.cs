using Npgsql.Age.Types;

namespace Graph.Api.Models;

public class Connection
{
    public Vertex FromVertex { get; set; }
    public Edge Edge { get; set; }
    public Vertex ToVertex { get; set; }
}
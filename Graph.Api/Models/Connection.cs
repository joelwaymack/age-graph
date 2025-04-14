namespace Graph.Api.Models;

public class Connection
{
    public GraphItem FromVertex { get; set; }
    public GraphItem Edge { get; set; }
    public GraphItem ToVertex { get; set; }
}
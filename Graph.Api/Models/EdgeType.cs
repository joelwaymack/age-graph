using System.Text.Json.Serialization;

namespace Graph.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter<EdgeType>))]
public enum EdgeType
{
    ConnectsTo,
    Contains,
}
namespace Graph.Api.DataAccess;

public record PostgresConfig(string ConnectionString, string GraphName)
{
    public static string ConfigSectionName => "Postgres";
}
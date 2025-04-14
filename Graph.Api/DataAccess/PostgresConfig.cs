namespace Graph.Api.DataAccess;

public record PostgresConfig(string ConnectionString, string DatabaseName, string GraphName)
{
    public static string ConfigSectionName => "Postgres";
}
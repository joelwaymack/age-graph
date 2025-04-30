namespace Graph.Api.Config;

public record AiConfig(string DeploymentName, string ApiKey, string Endpoint)
{
    public static string ConfigSectionName => "AiConfig";
}
using System.ComponentModel;
using Graph.Api.DataAccess;
using Microsoft.SemanticKernel;

namespace Graph.Api.Services;

public class AssetPlugin
{
    private readonly GraphDatabase _graphDatabase;
    private readonly ILogger<AssetPlugin> _logger;

    public AssetPlugin(GraphDatabase graphDatabase, ILogger<AssetPlugin> logger)
    {
        _graphDatabase = graphDatabase;
        _logger = logger;
    }

    [KernelFunction("get_asset_types")]
    [Description("Get all unique asset types")]
    public async Task<IList<string>> GetAssetTypesAsync()
    {
        try
        {
            var assetTypes = await _graphDatabase.ExecuteCypherQueryAsync<string>("MATCH (a:Asset) RETURN DISTINCT a.type");
            return assetTypes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset types.");
            throw;
        }
    }

}
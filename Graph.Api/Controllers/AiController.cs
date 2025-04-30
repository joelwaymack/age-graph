using Graph.Api.DataAccess;
using Graph.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Graph.Api.Controllers;

[ApiController]
[Route("ai")]
public class AiController : ControllerBase
{
    private readonly ILogger<AssetController> _logger;
    private readonly Kernel _kernel;

    public AiController(ILogger<AssetController> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Message cannot be null or empty.");
        }

        try
        {
            var history = new ChatHistory();
            history.AddSystemMessage("You help users find information about assets. Limit your response to data you can access through the Assets plugin.");
            history.AddUserMessage(request.Content);
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            var result = await chatCompletionService.GetChatMessageContentAsync(history, new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }, _kernel);
            return Ok(result.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request.");
            return StatusCode(500, "Internal server error.");
        }
    }
}
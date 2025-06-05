using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
// Removed: using Microsoft.SemanticKernel.AI; // Not directly needed for ChatMessageContent anymore

namespace OllamaWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")] // API endpoint will be /ollama
    public class OllamaController : ControllerBase
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;

        public OllamaController(IChatCompletionService chatCompletionService, Kernel kernel)
        {
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
        }

        // NEW: Intermediate DTO for chat messages to match frontend JSON structure
        public class SimpleChatMessage
        {
            public string? Role { get; set; }    // Will be "user" or "assistant" string
            public string? Content { get; set; } // The message content
        }

        // Modified: ChatRequest now uses SimpleChatMessage for history
        public class ChatRequest
        {
            public string? Message { get; set; }
            public List<SimpleChatMessage>? History { get; set; } // Now matches frontend
        }

        // DTO for outgoing chat responses (remains the same)
        public class ChatResponse
        {
            public string? Reply { get; set; }
        }

        /// <summary>
        /// Sends a message to the local Ollama SLM and gets a response, maintaining chat history.
        /// </summary>
        /// <param name="request">The chat request containing the user's current message and optional prior history.</param>
        /// <returns>The SLM's generated response.</returns>
        [HttpPost("chat")] // API endpoint will be /ollama/chat
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            ChatHistory chat = new("You are a helpful and concise AI assistant. Respond briefly and factually.");

            // Reconstruct previous conversation history from the request,
            // mapping SimpleChatMessage to Semantic Kernel's ChatMessageContent
            if (request.History != null)
            {
                foreach (var historyMessage in request.History)
                {
                    // Manually parse the string role to the AuthorRole enum
                    AuthorRole authorRole = historyMessage.Role?.ToLowerInvariant() switch
                    {
                        "user" => AuthorRole.User,
                        "assistant" => AuthorRole.Assistant,
                        "system" => AuthorRole.System, // Include if you send system messages from frontend
                        _ => AuthorRole.User // Default or handle unknown roles as needed
                    };
                    chat.AddMessage(authorRole, historyMessage.Content);
                }
            }

            // Add the current user's message to the chat history
            chat.AddUserMessage(request.Message);

            try
            {
                var result = await _chatCompletionService.GetChatMessageContentAsync(chat, kernel: _kernel);

                return Ok(new ChatResponse { Reply = result.Content });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with Ollama: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}. Check if Ollama is running and model is loaded.");
            }
        }

        // The /ollama/generate endpoint (GET) is not affected as it doesn't use history.
        // [HttpGet("generate")]
        // public async Task<ActionResult<ChatResponse>> Generate([FromQuery] string prompt)
        // { ... }
    }
}
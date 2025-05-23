using System.Text.Json.Nodes;
using GroqApiLibrary;

namespace GeekStore.API.Services
{
    public class GroqLLMService : ILLMService
    {
        private readonly GroqApiClient _groqApiClient;
        private readonly string _llmModel = Environment.GetEnvironmentVariable("GroqLLMModel");
        public GroqLLMService(GroqApiClient groqApiClient)
        {
            _groqApiClient = groqApiClient;
        }
        public async Task<string> GenerateRecommendationAsync(string text)
        {
            string embeddingSystemPrompt = "";
            
            var request = new JsonObject
            {
                ["model"] = _llmModel,
                ["max_tokens"] = 2048,
                ["temperature"] = 0,
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "system",
                        ["content"] = embeddingSystemPrompt
                    },
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = text
                    }
                }

            };

            var result = await _groqApiClient.CreateChatCompletionAsync(request);
            string? embeddingString = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            if(embeddingString == null)
            {
                throw new Exception("Failed to generate embedding");
            }

            return embeddingString;
        }
    }
}

using System.Text.Json.Nodes;
using GeekStore.API.Models.Domains;
using GeekStore.API.Services.Interfaces;
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
        public async Task<string> GenerateRecommendationAsync(string systemPrompt, string query, string availableProducts)
        {
            var request = new JsonObject
            {
                ["model"] = _llmModel,
                ["temperature"] = 0,
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "system",
                        ["content"] = systemPrompt
                    },
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $"User query: {query} \nAvailable products: {availableProducts}"
                    }
                }

            };

            var result = await _groqApiClient.CreateChatCompletionAsync(request);
            string? recommededProducts = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            if(recommededProducts == null)
            {
                throw new Exception("Failed to generate embedding");
            }

            return recommededProducts;
        }
    }
}

using GeekStore.API.Models.Domains;

namespace GeekStore.API.Services.Interfaces
{
    public interface ILLMService
    {
        Task<string> GenerateRecommendationAsync(string systemPrompt, string query, string availableProducts);
    }
}

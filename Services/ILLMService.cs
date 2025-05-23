namespace GeekStore.API.Services
{
    public interface ILLMService
    {
        Task<string> GenerateRecommendationAsync(string text);
    }
}

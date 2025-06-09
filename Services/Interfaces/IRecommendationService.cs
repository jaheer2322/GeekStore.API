using GeekStore.API.Models.DTOs;

namespace GeekStore.API.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<RecommendationsDto?> GetRecommendationAsync(RecommendationQueryDto queryDTO);
    }
}

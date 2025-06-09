using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class RecommendationQueryDto
    {
        [MinLength(24)]
        public string Query { get; set; }
        public bool IsExplanationNeeded { get; set; } = false;
    }
}

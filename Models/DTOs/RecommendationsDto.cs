namespace GeekStore.API.Models.DTOs
{
    public class RecommendationsDto
    {
        public string Message { get; set; }
        public List<BuildDto> Builds { get; set; }
        public string? Explanation { get; set; }
    }
}

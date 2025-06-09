namespace GeekStore.API.Models.DTOs
{
    public class BuildDto
    {
        public Dictionary<string, RecommendedProductDto> Parts { get; set; }
        public double totalBuildPrice { get; set; }
    }
}

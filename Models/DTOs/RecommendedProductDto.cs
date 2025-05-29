namespace GeekStore.API.Models.DTOs
{
    public class RecommendedProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
    }
}

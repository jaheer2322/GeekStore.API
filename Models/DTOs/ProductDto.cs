using GeekStore.API.Models.Domains;

namespace GeekStore.API.Models.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        // Nav props
        public Tier Tier { get; set; }
        public Category Category { get; set; }

    }
}

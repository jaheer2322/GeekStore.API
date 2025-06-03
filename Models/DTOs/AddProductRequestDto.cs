using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class AddProductRequestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [Range(1, 1000000)]
        public double Price { get; set; }
        [MinLength(1)]
        [MaxLength(1000)]
        public string? Description { get; set; }
        [MinLength(10)]
        [MaxLength(5000)]
        public string? Review { get; set; }
        [Required]
        public Guid TierId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
}

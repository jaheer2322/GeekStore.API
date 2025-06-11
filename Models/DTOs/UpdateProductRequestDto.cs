using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class UpdateProductRequestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }
        [MinLength(1)]
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        [Range(1, 1000000)]
        public double Price { get; set; }
        [Required]
        public Guid TierId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class AddTierRequestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}

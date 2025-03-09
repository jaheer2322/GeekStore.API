using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string name { get; set; }
    }
}

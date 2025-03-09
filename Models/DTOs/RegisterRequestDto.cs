using System.ComponentModel.DataAnnotations;

namespace GeekStore.API.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName {  get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string[] Roles { get; set; }
    }
}

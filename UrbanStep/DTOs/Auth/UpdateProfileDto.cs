using System.ComponentModel.DataAnnotations;

namespace UrbanStep.DTOs.Auth
{
    public class UpdateProfileDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace UrbanStep.DTOs.Auth
{
    public class PromoteToAdminDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

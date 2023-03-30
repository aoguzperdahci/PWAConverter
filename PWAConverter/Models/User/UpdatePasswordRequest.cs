using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.User
{
    public class UpdatePasswordRequest
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}

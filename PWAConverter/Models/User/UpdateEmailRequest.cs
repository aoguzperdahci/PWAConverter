using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.User
{
    public class UpdateEmailRequest
    {
        [Required]
        public string NewEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

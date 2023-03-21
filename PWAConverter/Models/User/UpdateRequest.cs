namespace PWAConverter.Models.User
{
    public class UpdateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}

namespace PWAConverter.Models.User
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
    }
}

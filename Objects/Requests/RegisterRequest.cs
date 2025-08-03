namespace SimpleLoginSystem.Objects.Requests
{
    public class RegisterRequest : IRequests
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}

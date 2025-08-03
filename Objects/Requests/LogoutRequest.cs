namespace SimpleLoginSystem.Objects.Requests
{
    public class LogoutRequest : IRequests
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}

namespace SimpleLoginSystem.Objects.Requests
{
    public class EditPassRequest : IRequests
    {
        public string? Email { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? Token { get; set; }
    }
}

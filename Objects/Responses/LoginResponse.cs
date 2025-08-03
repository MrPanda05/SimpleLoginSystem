namespace SimpleLoginSystem.Objects.Responses
{
    public class LoginResponse : IResponses
    {
        public UserDTO? UserDTO { get; set; }

        public string? Token { get; set; }
    }
}

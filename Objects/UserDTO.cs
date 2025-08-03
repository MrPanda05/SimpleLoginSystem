namespace SimpleLoginSystem.Objects
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsLoggedIn { get; set; } = false;
    }
}

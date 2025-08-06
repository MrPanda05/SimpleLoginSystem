namespace SimpleLoginSystem.Objects
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsLoggedIn { get; set; } = false;//not the ideal but simple enough
        public string? Role { get; set; } = "common";
        public string? Token { get; set; }

    }
}

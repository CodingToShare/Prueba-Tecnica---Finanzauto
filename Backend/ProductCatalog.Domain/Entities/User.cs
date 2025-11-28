namespace ProductCatalog.Domain.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Admin or User
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

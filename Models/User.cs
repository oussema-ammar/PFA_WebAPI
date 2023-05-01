namespace PFA_WebAPI.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } 
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}

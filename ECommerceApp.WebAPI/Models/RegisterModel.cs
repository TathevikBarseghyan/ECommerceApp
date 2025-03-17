namespace ECommerceApp.WebAPI.Models
{
    public class RegisterModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FullName { get; set; } = default!;

        public string? Role { get; set; }
    }
}

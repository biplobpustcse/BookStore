using System.ComponentModel.DataAnnotations;

namespace BookStore.Data.Models
{
    public class Registration
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage ="Name is required")]
        public string? Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; } = "User";
    }
}

using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Password { get; set; }

        public Uri ReturnUrl { get; set; } = new Uri("https://localhost/");
    }
}

namespace SportsStore.Models.ViewModels
{
    public class CartViewModel
    {
        public Cart? Cart { get; set; } = new();

        public Uri ReturnUrl { get; set; } = new Uri("https://localhost/");
    }
}

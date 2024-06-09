using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.Repository;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
  public class CartController : Controller
    {
        private readonly IStoreRepository repository;
        private readonly IConfiguration configuration;

        public CartController(IStoreRepository repository, Cart cart, IConfiguration configuration)
        {
            this.repository = repository;
            this.Cart = cart;
            this.configuration = configuration;
        }

        public Cart Cart { get; set; }

        [HttpGet]
        public IActionResult Index(Uri returnUrl)
        {
            string applicationUrl = this.configuration["ApplicationSettings:DefaultReturnUrl"] ?? string.Empty;
            return this.View(new CartViewModel
            {
                ReturnUrl = returnUrl ?? new Uri(applicationUrl),
                Cart = this.HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(),
            });
        }

        [HttpPost]
        public IActionResult Index(long productId, Uri returnUrl)
        {
            Product? product = this.repository.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                this.Cart.AddItem(product, 1);
                string applicationUrl = this.configuration["ApplicationSettings:DefaultReturnUrl"] ?? string.Empty;
                ValidateDefaultUri(applicationUrl);
                return this.View(new CartViewModel
                {
                    Cart = this.Cart,
                    ReturnUrl = returnUrl ?? new Uri(applicationUrl),
                });
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Cart/Remove")]
        public IActionResult Remove(long productId, Uri returnUrl)
        {
            this.Cart.RemoveLine(this.Cart.Lines.First(cl => cl.Product.ProductId == productId).Product);
            string applicationUrl = this.configuration["ApplicationSettings:DefaultReturnUrl"] ?? string.Empty;
            ValidateDefaultUri(applicationUrl);
            return this.View("Index", new CartViewModel
            {
                Cart = this.Cart,
                ReturnUrl = returnUrl ?? new Uri(applicationUrl),
            });
        }

        private static void ValidateDefaultUri(string applicationUrl)
        {
            if (string.IsNullOrEmpty(applicationUrl))
            {
                throw new ArgumentNullException(nameof(applicationUrl));
            }
        }
    }
}

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

        public CartController(IStoreRepository repository, Cart cart)
        {
            this.repository = repository;
            this.Cart = cart;
        }

        public Cart Cart { get; set; }

        [HttpGet]
        public IActionResult Index(Uri returnUrl)
        {
            return this.View(new CartViewModel
            {
                ReturnUrl = returnUrl ?? new Uri("https://localhost/"),
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

                return this.View(new CartViewModel
                {
                    Cart = this.Cart,
                    ReturnUrl = returnUrl ?? new Uri("https://localhost/"),
                });
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Cart/Remove")]
        public IActionResult Remove(long productId, Uri returnUrl)
        {
            this.Cart.RemoveLine(this.Cart.Lines.First(cl => cl.Product.ProductId == productId).Product);
            return this.View("Index", new CartViewModel
            {
                Cart = this.Cart,
                ReturnUrl = returnUrl ?? new Uri("https://localhost/"),
            });
        }
    }
}

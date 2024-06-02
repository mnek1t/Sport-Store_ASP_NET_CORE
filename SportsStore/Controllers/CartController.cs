using Microsoft.AspNetCore.Mvc;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.Repository;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
  public class CartController : Controller
    {
        private IStoreRepository repository;

        public CartController(IStoreRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            return View(new CartViewModel
            {
                ReturnUrl = returnUrl ?? "/",
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(),
            });

        }

        [HttpPost]
        public IActionResult Index(long productId, string returnUrl)
        {
            Product? product = repository.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
                return View(new CartViewModel { Cart = cart, ReturnUrl = returnUrl });
            }

            return RedirectToAction("Index", "Home");
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly Cart cart;

        public OrderController(IOrderRepository orderRepository, Cart cart)
        {
            this.orderRepository = orderRepository;
            this.cart = cart;
        }

        public ViewResult Checkout() => this.View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (order == null) 
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (!this.cart.Lines.Any())
            {
                this.ModelState.AddModelError(key: string.Empty, errorMessage: "Sorry, your cart is empty!");
            }

            if (this.ModelState.IsValid)
            {
                order.SetLines(this.cart.Lines.ToArray());
                this.orderRepository.SaveOrder(order: order);
                this.cart.Clear();
                return this.View(viewName: "Completed", model: order.OrderId);
            }

            return this.View();
        }
    }
}

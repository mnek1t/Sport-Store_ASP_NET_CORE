using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        public ViewResult Checkout() => View(new Order());
    }

}

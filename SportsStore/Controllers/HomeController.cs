using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository repository;
        public HomeController(IStoreRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult Index() => View(repository.Products);
    }
}

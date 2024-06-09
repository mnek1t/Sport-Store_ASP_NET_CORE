using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Repository;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository repository;

        private readonly int pageSize = 4;

        public HomeController(IStoreRepository repository)
        {
            this.repository = repository;
        }

        public ViewResult Index(string? category, int productPage = 1)
              => this.View(new ProductsListViewModel
              {
                  Products = this.repository.Products
                  .Where(p => category == null || p.Category == category)
                  .OrderBy(p => p.ProductId)
                  .Skip((productPage - 1) * this.pageSize)
                  .Take(this.pageSize),
                  PagingInfo = new PagingInfo
                  {
                      CurrentPage = productPage,
                      ItemsPerPage = this.pageSize,
                      TotalItems = this.repository.Products.Count(),
                  },
                  CurrentCategory = category,
              });

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}

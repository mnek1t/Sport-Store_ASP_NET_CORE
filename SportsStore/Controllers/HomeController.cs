using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Repository;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository repository;
        public HomeController(IStoreRepository repository)
        {
            this.repository = repository;
        }

        public int PageSize = 4;
        public ViewResult Index(int productPage = 1)
        {
            return View(new ProductsListViewModel
            {
                Products = repository.Products
                               .OrderBy(p => p.ProductId)
                               .Skip((productPage - 1) * PageSize)
                               .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Products.Count(),
                },
            });

        }
    }
}

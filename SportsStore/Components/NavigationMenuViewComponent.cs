using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Repository;

namespace SportsStore.Components
{
  public class NavigationMenuViewComponent : ViewComponent
  {
        private IStoreRepository repository;
        public NavigationMenuViewComponent(IStoreRepository repository)
        {
            this.repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(repository.Products
                   .Select(x => x.Category)
                   .Distinct()
                   .OrderBy(x => x));
        }
    }
}

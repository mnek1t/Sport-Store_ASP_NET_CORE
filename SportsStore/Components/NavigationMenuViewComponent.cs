using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Repository;

namespace SportsStore.Components
{
  public class NavigationMenuViewComponent : ViewComponent
  {
        private readonly IStoreRepository repository;

        public NavigationMenuViewComponent(IStoreRepository repository)
        {
            this.repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            this.ViewBag.SelectedCategory = this.RouteData?.Values["category"];
            return this.View(this.repository.Products
                   .Select(x => x.Category)
                   .Distinct()
                   .OrderBy(x => x));
        }
    }
}

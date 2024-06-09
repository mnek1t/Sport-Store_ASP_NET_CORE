using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Components
{
    public class AdminNavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            this.ViewBag.Selection = this.Request.Path.Value ?? "Products";

            return this.View(new List<string> { "Orders", "Products" });
        }
    }
}

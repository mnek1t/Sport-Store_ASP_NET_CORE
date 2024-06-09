using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Components
{
    public class AdminNavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.Selection = Request.Path.Value ?? "Products";

            return View(new List<string> { "Orders", "Products" });
        }
    }
}

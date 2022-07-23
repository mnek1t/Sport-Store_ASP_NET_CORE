#  Sport Store Application. Part 2

## Implementation details

- [Adding Navigation Controls](#adding-navigation-controls)
- [Building the Shopping Cart](#building-the-shopping-cart)

###  Adding Navigation Controls

- Go to the cloned repository of the previous step `Part 1`. 

- Switch to the `sports-store-application-2` branch.

```
    $ git checkout sports-store-application-2

    $ git merge main

```
- Switch to the `sports-store-application-2` branch and do a fast-forward merge according to changes from the main branch.

- Modify the `ProductsListViewModel` class (add the `CurrentCategory` property).

```
namespace SportsStore.Models.ViewModels 
{
    public class ProductsListViewModel 
    {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}
```

- Add the `Category` support to the `HomeController` class.

```
public ViewResult Index(string category, int productPage = 1)
=> View(new ProductsListViewModel 
{
    Products = repository.Products
        .Where(p => category == null || p.Category == category)
        .OrderBy(p => p.ProductID)
        .Skip((productPage - 1) * PageSize)
        .Take(PageSize),
    PagingInfo = new PagingInfo {
       CurrentPage = productPage,
       ItemsPerPage = PageSize,
       TotalItems = repository.Products.Count()
    },
    CurrentCategory = category
});
```

- Restart ASP.NET Core and select a category using the following URL http://localhost:5000/?category=Soccer. Make sure to use an uppercase `S` in `Soccer`.

- To improve the URLs like `/?category = Soccer`, change the routing configuration in the Configure method of the Startup class. Create a more useful set of URLs. It is important to add the new routes in the order they are shown.

```
...
app.UseRouting();
app.UseEndpoints(endpoints => 
{
    endpoints.MapControllerRoute("catpage",
        "{category}/Page{productPage:int}",
        new { Controller = "Home", action = "Index" });
    endpoints.MapControllerRoute("page", "Page{productPage:int}",
        new { Controller = "Home", action = "Index", productPage = 1 });
    endpoints.MapControllerRoute("category", "{category}",
        new { Controller = "Home", action = "Index", productPage = 1 });
    endpoints.MapControllerRoute("pagination",
        "Products/Page{productPage}",
        new { Controller = "Home", action = "Index", productPage = 1 });
    endpoints.MapDefaultControllerRoute();
});
SeedData.EnsurePopulated(app);
...
```

| URL | Leads to | Picture
| ------ | ------ | ------ |
| / | Shows the first page of products from all categories | #1 |
| /Page2 | Shows the specified page (in this case, page 2), showing items from all categories | #2 |
| /Soccer | Shows the first page of items from a specific category (in this case, the Soccer category) | #3 |
| /Soccer/Page1 | Shows the specified page (in this case, page 1) of items from the specified category (in this case, Soccer) | #4 |
| /Chess/Page1 | Shows the specified page (in this case, page 1) of items from the specified category (in this case, Chess) | #5 |

Image #1   
![](Images/1.1.png)
Image #2  
![](Images/6.1.png)
Image #3 
![](Images/7.1.png)
Image #4  
![](Images/8.1.png)
Image #5 
![](Images/3.1.png)

    
- To start generating more complex URLs, it's necessary to receive additional information from the view without having to add extra properties to the tag helper class. Add `Prefixed Values` in the `PageLinkTagHelper` to receive properties with a common prefix all together in a single collection.

```
[HtmlTargetElement("div", Attributes = "page-model")]
public class PageLinkTagHelper : TagHelper 
{
    ...
    [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
    public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();
    ...
    public override void Process(TagHelperContext context, TagHelperOutput output) 
    {
        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext)  
        TagBuilder result = new TagBuilder("div");
        for (int i = 1; i <= PageModel.TotalPages; i++) 
        {
            TagBuilder tag = new TagBuilder("a");
            PageUrlValues["productPage"] = i;
            tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled) 
            {
                tag.AddCssClass(PageClass);
                tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
            }
            tag.InnerHtml.Append(i.ToString());
            result.InnerHtml.AppendHtml(tag);
        
        output.Content.AppendHtml(result.InnerHtml);
    }
}

```
- Add a new attribute in the `Index.cshtml` file in the `SportsStore/Views/Home` folder.

```
@model ProductsListViewMode
@foreach (var p in Model.Products) 
{
    <partial name="ProductSummary" model="p" />

<div page-model="@Model.PagingInfo" page-action="Index" page-classes-enabled="true"
     page-class="btn" page-class-normal="btn-outline-dark"
     page-class-selected="btn-primary" page-url-category="@Model.CurrentCategory"
     class="btn-group pull-right m-1">
</div>
```

- Restart ASP.NET Core and request http://localhost:5000/Chess.

-  Сreate a folder called `Components`, which is the conventional home of view components, in the `SportsStore` project.

-  Add the `NavigationMenuViewComponent` class to it.

```
public class NavigationMenuViewComponent : ViewComponent 
{
    public string Invoke() 
    {
        return "Hello from the Nav View Component";
    }
}
```

- To view the result of the `Invoke` method, open the  `_Layout.cshtml` file and add the tag `<vc:navigation-menu />` as shown below: 

```
...
 <div class="row m-1 p-1">
    <div id="categories" class="col-3">
        <vc:navigation-menu />
    </div>
    <div class="col-9">
        @RenderBody()
    </div>
</div>
...
```
    
- Restart ASP.NET Core and request http://localhost:5000`.

- Change the `NavigationMenuViewComponent` class, add categories:

```
public class NavigationMenuViewComponent : ViewComponent 
{
    private IStoreRepository  repository
    public NavigationMenuViewComponent(IStoreRepository repo) 
    {
        repository = repo;
    
    public IViewComponentResult Invoke() 
    {
        return View(repository.Products
           .Select(x => x.Category)
           .Distinct()
           .OrderBy(x => x));
    }
}
```

- Create the `Views/Shared/Components/NavigationMenu` folder in the `SportsStore` project and add it to the Razor view named `Default.cshtml`.

```
@model IEnumerable<string
<a class="btn btn-block btn-outline-secondary" 
    asp-action="Index" 
    asp-controller="Home" 
    asp-route-category="">
    Home
</a
@foreach (string category in Model) 
{
    <a class="btn btn-block btn-outline-secondary"
       asp-action="Index" asp-controller="Home"
       asp-route-category="@category"
       asp-route-productPage="1">
        @category
    </a>
}
```

- Restart ASP.NET Core and request http://localhost:5000.

- Use the `RouteData` property in the `Invoke` method of `NavigationMenuViewComponent` to access the requested data in order to get the value for the currently selected category. 

```
public class NavigationMenuViewComponent : ViewComponent 
{
        ...
        public IViewComponentResult Invoke() 
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            ...
        }
        ...
    }
}
```

- To highlight the selected categories, change the `Default.cshtml` file.

```
@foreach (string category in Model) 
{
    <a class="btn btn-block
        @(category == ViewBag.SelectedCategory ? "btn-primary": "btn-outline-secondary")"
        asp-action="Index" asp-controller="Home"
        asp-route-category="@category"
        asp-route-productPage="1">
        @category
    </a>
}
...
```
- Restart ASP.NET Core and request http://localhost:5000.

- Update the `Index` action method in the `Home` controller which will allow you to take into account the categories in the pagination (the functionality that breaks the selection result into pages). 

```
public ViewResult Index(string category, int productPage = 1)
         => View(new ProductsListViewModel 
            {
                Products = repository.Products
                   .Where(p => category == null || p.Category == category)
                   .OrderBy(p => p.ProductID)
                   .Skip((productPage - 1) * PageSize)
                   .Take(PageSize),
                PagingInfo = new PagingInfo {
                   CurrentPage = productPage,
                   ItemsPerPage = PageSize,
                   TotalItems = category == null ?
                       repository.Products.Count() :
                       repository.Products.Where(e =>
                           e.Category == category).Count()
               },
               CurrentCategory = category
            });
        
```

- Restart ASP.NET Core and request http://localhost:5000.

- Commit changes.

###  Building the Shopping Cart

- Add a new `_CartLayout.cshtml` file in the `SportsStore/Views/Shared` folder for the`Cart` views.

```
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>SportsStore</title>
    <link href="/lib/twitter-bootstrap/css/bootstrap.min.css" rel="stylesheet" />    
</head>
<body>
    <div class="bg-dark text-white p-2">
        <span class="navbar-brand ml-2">SPORTS STORE</span>
    </div>
    <div class="m-1 p-1">
            @RenderBody()
    </div>
</body>
</html>
```

- Add the `CartController` class in the `SportsStore/Controllers` folder.

```
public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

- Add the `Index.cshtml` file in the `SportsStore/Views/Cart` folder.

```
@{
    this.Layout = "_CartLayout";
}

<h4>This is the Cart Page</h4>
```


- Restart ASP.NET Core and request http://localhost:5000.

    ![](Images/9.1.png)

- To create the buttons that will add products to the cart, add the `UrlExtensions` class (`Infrastructure` folder) and define the `PathAndQuery` extension method in it.

```
public static class UrlExtensions
{
    public static string PathAndQuery(this HttpRequest request) 
        => request.QueryString.HasValue ? $"{request.Path}{request.QueryString}" : request.Path.ToString();
}
```

The extension method generates a URL. The browser will return to this URL after the cart has been updated. If there are Query Parameters in the URL, they should be considered as well.  

- Add the markup for the buttons into the view `ProductSummary.cshtml` within the `SportsStore/Views/Shared` folder.
        
```
@model Product

<div class="card card-outline-primary m-1 p-1">

    <div class="bg-faded p-1">
        <h4>
            @Model.Name
            <span class="badge badge-pill badge-primary" style="float:right">
                <small>@Model.Price.ToString("c")</small>
            </span>
        </h4>
    </div>
    
    <form id="@Model.ProductId" method="post" asp-action="Index" asp-controller="Cart"  asp-antiforgery="true">
        <input type="hidden" asp-for="ProductId"/>
        <input type="hidden" name="returnUrl"
               value="@ViewContext.HttpContext.Request.PathAndQuery()"/>
        <span class="card-text p-1">
            @Model.Description
            <button type="submit" class="btn btn-success btn-sm pull-right" style="float:right">
                Add To Cart
            </button>
        </span>
    </form>

</div>
```

- Use the session state mechanism to store information about a user’s cart. In order to do this, add services and middleware to the `Startup` class (`AddSession()` and `UseSession()` methods).

```
...
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddDistributedMemoryCache();
    services.AddSession();
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
{
    ...
    app.UseStaticFiles();
    app.UseSession();
    app.UseRouting();
    ...
}
...
```


- To implement the cart feature, add the `Cart`class and the `CartLine` class (in the `Models` folder) in the `SportsStore` project. 

```
public class Cart
{
    public List<CartLine> Lines { get; set; } = new List<CartLine>()
    public void AddItem(Product product, int quantity)
    {
        CartLine viewModel = Lines.FirstOrDefault(p => p.Product.ProductId == product.ProductId)
        if (viewModel == null)
        {
            Lines.Add(new CartLine
            {
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            viewModel.Quantity += quantity;
        }
    }

    public void RemoveLine(Product product) =>
        Lines.RemoveAll(l => l.Product.ProductId == product.ProductId); 

    public decimal ComputeTotalValue() =>
        Lines.Sum(e => e.Product.Price * e.Quantity);

    public void Clear() => Lines.Clear();
}

public class CartLine
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
}
```


The `Cart` class uses the `CartLine` class to represent a product selected by the customer and the quantity a user wants to buy. The Cart class includes the methods that add an item to the cart, remove a previously added item from the cart, calculate the total cost of the items in the cart, and reset the cart by removing all the items.

- To store a `Cart` object (the session state feature in ASP.NET Core stores only int, string, and byte[] values) define extension methods to the `ISession` interface that provides access to the session state data to serialize `Cart` objects into JSON and convert them back. Add the ion interface that provides access to the session state data to serialize Cart objects into JSON and convert them back. Add the `SessionExtensions` class (the `Infrastructure` folder) and defined the extension methods. 

```
public static class SessionExtensions
{
    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
    
    public static T GetJson<T>(this ISession session, string key)
    {
        var sessionData = session.GetString(key);
        return sessionData == null ? default : JsonSerializer.Deserialize<T>(sessionData);
    }
}
```

- Add the `CartViewModel` class (the `SportsStore/Models/ViewModels` folder).

```
public class CartViewModel
{
    public Cart Cart { get; set; }
    public string ReturnUrl { get; set; }
}
```

- Change the `CartController` class:

        public class CartController : Controller
        {
            private IStoreRepository repository;

            public CartController(IStoreRepository repo)
            {
                repository = repo;
            }

            [HttpGet]
            public IActionResult Index(string returnUrl)
            {
                return View(new CartViewModel
                {
                    Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(),
                    ReturnUrl = returnUrl ?? "/"
                });
            }

            [HttpPost]
            public IActionResult Index(long productId, string returnUrl)
            {
                Product product = repository.Products.FirstOrDefault(p => p.ProductId == productId);
                var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
                return View(new CartViewModel
                {
                    Cart = cart,
                    ReturnUrl = returnUrl
                });
            }
        }

- Change the `Index.cshtml` file in the `SportsStore/Views/Cart` folder:

```
...

@model CartViewModel

<h2>Your cart</h2>
<table class="table table-bordered table-striped">
    <thead>
        <tr>
            <th>Quantity</th>
            <th>Item</th>
            <th class="text-right">Price</th>
            <th class="text-right">Subtotal</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var line in Model.Cart.Lines) {
            <tr>
                <td class="text-center">@line.Quantity</td>
                <td class="text-left">@line.Product.Name</td>
                <td class="text-right">@line.Product.Price.ToString("c")</td>
                <td class="text-right">
                    @((line.Quantity * line.Product.Price).ToString("c"))
                </td>
            </tr>
        }
    </tbody>
    <tfoot>
        <tr>
            <td colspan="3" class="text-right">Total:</td>
            <td class="text-right">
                @Model.Cart.ComputeTotalValue().ToString("c")
            </td>
        </tr>
    </tfoot>
</table>

<div class="text-center">
    <a class="btn btn-primary" href="@Model.ReturnUrl">Continue shopping</a>
</div>

```
- Restart ASP.NET Core and request http://localhost:5000. As a result, the basic functions of the shopping cart should be in place. First, products are listed along with the button that adds them to the cart. You can see that by restarting ASP.NET Core and requesting http://localhost:5000.  

![](Images/1.1.png)

Then, when the user clicks the `Add To Cart` button, the selected product is added to their cart:
    
![](Images/3.1.png)

and the summary of the cart is displayed, as shown below:

![](Images/4.1.png)

Clicking the `Continue Shopping button` returns the user to the product page they came from.

![](Images/10.1.png)

- Commit changes.

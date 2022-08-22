#  Creating "Sports Store" Application. Part 3

## Description

- [Refining the Cart Model with a Service](#refining-the-cart-model-with-a-service)
- [Completing the Cart Functionality](#completing-the-cart-functionality)
- [Submitting Orders](#submitting-orders)

## TODO

###  Refining the Cart Model with a Service

- Use SportsStore` ASP.NET Core MVC Application. Part 2

- To can override the members of the `Cart` class apply the `virtual` keyword to the `AddItem`, `RemoveLine`, `Clear` methods of the `Cart` class

- Add a `SessionCart` class  (int the `Models` folder)

        using System;
        using System.Text.Json.Serialization;
        using Microsoft.AspNetCore.Http;
        using Microsoft.Extensions.DependencyInjection;
        using SportsStore.Infrastructure;

        namespace SportsStore.Models
        {
            public class SessionCart : Cart
            {
                public static Cart GetCart(IServiceProvider services)
                {
                    ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
                    SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
                    cart.Session = session;
                    return cart;
                }

                [JsonIgnore] public ISession Session { get; set; }

                public override void AddItem(Product product, int quantity)
                {
                    base.AddItem(product, quantity);
                    Session.SetJson("Cart", this);
                }

                public override void RemoveLine(Product product)
                {
                    base.RemoveLine(product);
                    Session.SetJson("Cart", this);
                }

                public override void Clear()
                {
                    base.Clear();
                    Session.Remove("Cart");
                }
            }
        }

-  Create a service for the `Cart` class

        public void ConfigureServices(IServiceCollection services) 
        {
            ...
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

- Simplify the `CartController` class where `Cart` objects are used

        public class CartController : Controller
        {
            private readonly IStoreRepository repository;
            private readonly Cart cart;

            public CartController(IStoreRepository repo, Cart cartService)
            {
                repository = repo;
                cart = cartService;
            }

            [HttpGet]
            public IActionResult Index(string returnUrl)
            {
                return View(new CartViewModel
                {
                    ReturnUrl = returnUrl ?? "/"
                });
            }

            [HttpPost]
            public IActionResult Index(long productId, string returnUrl)
            {
                Product product = repository.Products.FirstOrDefault(p => p.ProductId == productId);
                cart.AddItem(product, 1);
                return View(new CartViewModel
                {
                    Cart = cart,
                    ReturnUrl = returnUrl
                });
            }
        }

- Restart ASP.NET Core and request http://localhost:5000/

### Completing the Cart Functionality

- To remove items from the cart add to the `Index.cshtml` file a `Remove` button  that will submit an HTTP POST request (see `SportsStore/Views/Cart` folder)

        ...
        @foreach (var line in Model.Cart.Lines)
        {
            <tr>
                <td class="text-center">@line.Quantity</td>
                <td class="text-left">@line.Product.Name</td>
                <td class="text-right">@line.Product.Price.ToString("c")</td>
                <td class="text-right">
                    @((line.Quantity * line.Product.Price).ToString("c"))
                </td>
                <td class="text-center">
                    <form method="post" asp-action="Remove" asp-controller="Cart">
                        <input type="hidden" name="ProductID" value="@line.Product.ProductId"/>
                        <input type="hidden" name="returnUrl" value="@Model.ReturnUrl"/>
                        <button type="submit" class="btn btn-sm btn-danger">
                            Remove
                        </button>
                    </form>
                </td>
            </tr>
        }
        ...

- Add a `Remove` method to the `CartController` class

        [HttpPost]
        public IActionResult Remove(long productId, string returnUrl) 
        {
            cart.RemoveLine(cart.Lines.First(cl => cl.Product.ProductId == productId).Product);
            
            return View("Index", new CartViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl ?? "/"
            });
        }

- Restart ASP.NET Core and request http://localhost:5000/Cart

    ![](Images/3.1.png)

- Add a widget that summarizes the contents of the cart and that can be clicked to display the cart contents throughout the application. Use the `Font Awesome` package, which is an excellent set of open source icons that are integrated into applications as fonts, where each character in the font is a different image (see ) http://fortawesome.github.io/Font-Awesome). To install the client-side package, use a PowerShell command prompt to run the command

        libman install font-awesome@5.12.0 -d wwwroot/lib/font-awesome

- Add a `CartSummaryViewComponent` class (the `Components` folder)

        public class CartSummaryViewComponent : ViewComponent
        {
            private Cart cart;
            
            public CartSummaryViewComponent(Cart cartService) 
            {
                cart = cartService;
            }

            public IViewComponentResult Invoke() 
            {
                return View(cart);
            }
        }

- Created the `Views/Shared/Components/CartSummary` folder and add to it a View Component named `Default.cshtml` with the content

        @model Cart
        
        <div class="">
            @if (Model.Lines.Any())
            {
                <small class="navbar-text">
                    <b>Your cart:</b>
                    @Model.Lines.Sum(x => x.Quantity) item(s)
                    @Model.ComputeTotalValue().ToString("c")
                </small>
            }
            
            <a class="btn btn-sm btn-secondary navbar-btn" 
               asp-page="/Cart" 
               asp-route-returnurl="@ViewContext.HttpContext.Request.PathAndQuery()">
                <i class="fa fa-shopping-cart"></i>
            </a>
        </div>

- To display a button with the Font Awesome cart icon and, if there are items in the cart, provides a snapshot that details the number of items and their total value, adding the `Cart Summary` in the `_Layout.cshtml` file (the Views/Shared folder)

        <!DOCTYPE html>
        <html>
        <head>
            ...
            <link href="/lib/font-awesome/css/all.min.css" rel="stylesheet"/>
        </head>
        <body>
        <div class="bg-dark text-white p-2">
            <div class="container-fluid">
                <div class="row">
                    <div class="col navbar-brand">SPORTS STORE</div>
                    <div class="col-6 text-right">
                        <vc:cart-summary/>
                    </div>
                </div>
            </div>
        </div>
        ...
        </body>
        </html>

- Restart ASP.NET Core and request http://localhost:5000/

    ![](Images/3.2.png)

###  Submitting Orders

- To represent the shipping details for a customer add a `Order` class (the `Models` folder)

        public class Order
        {
            [BindNever] public int OrderID { get; set; }

            [BindNever] public ICollection<CartLine> Lines { get; set; }
    
            [Required(ErrorMessage = "Please enter a name")]
            public string Name { get; set; }
    
            [Required(ErrorMessage = "Please enter the first address line")]
            public string Line1 { get; set; }
    
            public string Line2 { get; set; }
            public string Line3 { get; set; }
    
            [Required(ErrorMessage = "Please enter a city name")]
            public string City { get; set; }
    
            [Required(ErrorMessage = "Please enter a state name")]
            public string State { get; set; }
    
            public string Zip { get; set; }
    
            [Required(ErrorMessage = "Please enter a country name")]
            public string Country { get; set; }
    
            public bool GiftWrap { get; set; }
        }

-  Add a `Checkout` button to the cart view (in the `Index.cshtml` file in the `SportsStore/Views/Cart` folder)

        ...
        <div class="text-center">
            <a class="btn btn-primary" href="@Model.ReturnUrl">Continue shopping</a>
            <a class="btn btn-primary" asp-action="Checkout" asp-controller="Order">
                Checkout
            </a>
        </div>

- Add a class `OrderController` (the `Controllers` folder) with a `Checkout` action method

        public class OrderController : Controller 
        {
            public ViewResult Checkout() => View(new Order());
        }

- Create the `Views/Order` folder and added to it a Razor View called `Checkout.cshtml`
        
        @model Order
        
        <h2>Check out now</h2>
        <p>Please enter your details, and we'll ship your goods right away!</p>
        
        <div asp-validation-summary="All" class="text-danger"></div>
        
        <form asp-action="Checkout" method="post">
            <h3>Ship to</h3>
            <div class="form-group">
                <label>Name:</label><input asp-for="Name" class="form-control" />
            </div>
            <h3>Address</h3>
            <div class="form-group">
                <label>Line 1:</label><input asp-for="Line1" class="form-control" />
            </div>
            <div class="form-group">
                <label>Line 2:</label><input asp-for="Line2" class="form-control" />
            </div>
            <div class="form-group">
                <label>Line 3:</label><input asp-for="Line3" class="form-control" />
            </div>
            <div class="form-group">
                <label>City:</label><input asp-for="City" class="form-control" />
            </div>
            <div class="form-group">
                <label>State:</label><input asp-for="State" class="form-control" />
            </div>
            <div class="form-group">
                <label>Zip:</label><input asp-for="Zip" class="form-control" />
            </div>
            <div class="form-group">
                <label>Country:</label><input asp-for="Country" class="form-control" />
            </div>
            <h3>Options</h3>
            <div class="checkbox">
                <label>
                    <input asp-for="GiftWrap" /> Gift wrap these items
                </label>
            </div>
            <div class="text-center">
                <input class="btn btn-primary" type="submit" value="Complete Order" />
            </div>
        </form>
        
- Restart ASP.NET Core and request http://localhost:5000/Order/Checkout 

    ![](Images/3.4.png)

#### Implementing Order Processing

- Add a new property to the `StoreDbContext` database context class (the `SportsStore/Models` folder)

        public class StoreDbContext : DbContext
        {
            ...
            public DbSet<Order> Orders { get; set; }
        }

-  To create the migration, use a PowerShell command prompt to run the command

        dotnet ef migrations add Orders

- Follow the same pattern that was used for the `Product` Repository for providing access to `Order` objects. Add the `IOrderRepository` interface (the `Models` folder)

        public interface IOrderRepository
        {
            IQueryable<Order> Orders { get; }
            void SaveOrder(Order order);
        }

- To implement the order repository interface,  add a `EFOrderRepository` class (the `Models` folder)

        public class EFOrderRepository : IOrderRepository
        {
            private StoreDbContext context;

            public EFOrderRepository(StoreDbContext ctx)
            {
                context = ctx;
            }

            public IQueryable<Order> Orders => context.Orders
                .Include(o => o.Lines)
                .ThenInclude(l => l.Product);

            public void SaveOrder(Order order)
            {
                context.AttachRange(order.Lines.Select(l => l.Product));
                if (order.OrderID == 0)
                {
                    context.Orders.Add(order);
                }

                context.SaveChanges();
            }
        }

    This class implements the IOrderRepository interface using Entity Framework Core, allowing the set of Order objects that have been stored to be retrieved and allowing for orders to be created or changed.

- Register the `Order Repository Service` in the `Startup` class

        public void ConfigureServices(IServiceCollection services) 
        {
            ...
            services.AddScoped<IOrderRepository, EFOrderRepository>();
            ...
        }
    
- To complete the `OrderController` class modify the constructor so that it receives the services it requires to process an order and add an action method that will handle the HTTP form POST request when the user clicks the Complete Order button 

        public class OrderController : Controller
        {
            private IOrderRepository repository;

            private Cart cart;

            public OrderController(IOrderRepository repoService, Cart cartService)
            {
                repository = repoService;
                cart = cartService;
            }

            [HttpGet]
            public ViewResult Checkout() => View(new Order());

            [HttpPost]
            public IActionResult Checkout(Order order)
            {
                if (!cart.Lines.Any())
                {
                    ModelState.AddModelError("", "Sorry, your cart is empty!");
                }

                if (ModelState.IsValid)
                {
                    order.Lines = cart.Lines.ToArray();
                    repository.SaveOrder(order);
                    cart.Clear();
                    return View("Completed", order.OrderID);
                }

                return View();
            }
        }

- To complete the checkout process, create a `Completed.cshtml` Razor Page that displays a thank-you message with a summary of the orders

        @model int

        @{
            this.Layout = "_CartLayout";
        }

        <div class="text-center">
            <h2>Thanks!</h2>
            <p>Thanks for placing order #@Model.</p>
            <p>We'll ship your goods as soon as possible.</p>
            <a class="btn btn-primary" asp-controller="Home" asp-action="Index">Return to Store</a>
        </div>

- Restart ASP.NET Core and request http://localhost:5000/Order/Checkout 

     ![](Images/3.3.png)
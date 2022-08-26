# Sports Store Application. Part 4 (in progress)

## Implementation details

<details>
<summary>

**Managing Orders**
</summary>

- Go to the cloned repository of the previous step `Sport Store Application. Part 3`. 

- Switch to the `sports-store-application-4` branch and do a fast-forward merge according to changes from the `main` branch.

```
$ git checkout sports-store-application-4

$ git merge main --ff

```
- Continue your work in Visual Studio or other IDE.

- Build project, run application and request http://localhost:5000/. Your application should be work.

- Create and add to `Controllers` folder a separate `AdminController.cs` controller for managing orders shipping and the product catalog

```
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class AdminController : Controller
    {
        private IStoreRepository storeRepository;
        private IOrderRepository orderRepository;

        public AdminController(IStoreRepository storeRepository, IOrderRepository orderRepository) 
            => (this.storeRepository, this.orderRepository) = (storeRepository, orderRepository);

        public ViewResult Orders() => View(orderRepository.Orders);

        public ViewResult Products() => View(storeRepository.Products);
    }
}
```
- Add the `AdminNavigationMenuViewComponent` class to `Components` folder

```
using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Components
{
    public class AdminNavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedAction = RouteData?.Values["action"];

            return View(new string[] { "Orders", "Products" });
        }
    }
}
```
- Add the `Default.cshtml` Razor view named  to `Views/Shared/Components/AdminNavigationMenu` folder 

```
<div class="d-grid gap-2">
    @foreach (string category in Model)
    {
        <a class="btn @(category == ViewBag.SelectedAction ? "btn-primary" : "btn-outline-secondary")"
        asp-action="@category" asp-controller="Admin">
            @category
        </a>
    }
</div>
```
- To create the layout for the administration tools, add a `_AdminLayout.cshtml` Layout View with the content shown below to the `Views/Admin` folder 
```
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>SportsStore</title>
    <link href="/lib/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <div class="bg-info text-white p-2">
        <div class="container-fluid">
            <span class="navbar-brand">SPORTS STORE Administration</span>
        </div>
    </div>
    <div class="container-fluid">
        <div class="row p-2">
            <div class="col-3">
                <vc:admin-navigation-menu />
            </div>
            <div class="col-9">
                @RenderBody()
            </div>
        </div>
    </div>
</body>
</html>
```
- To complete the initial setup, add the views that will provide the administration tools, although they will contain placeholder messages at first. Add a `Orders.cshtml` View to the `Views/Admin` folder with the content shown below

```
@model IQueryable<Order>

@{
    Layout = "_AdminLayout";
}

<h4>This is the orders information.</h4>
```
and add a `Products.cshtml` View to the `Views/Admin` folder with the content shown below

```
@model IQueryable<Product>

@{
    Layout = "_AdminLayout";
}

<h4>This is the products information.</h4>

```
- Build project, run application and request http://localhost:5000/Admin/Orders 

![](Images/4.1.png)

and http://localhost:5000/Admin/Products

![](Images/4.2.png)

- To create a simple administration tool that will let to view the orders that have been received and mark them as shipped, at first change the data model so that adminstator can record which orders have been shipped. Add a `Shipped` property in the `Order.cs` file (the `Models` Folder)

```
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SportsStore.Models
{
    public class Order
    {
        . . .

        [BindNever]
        public bool Shipped { get; set; }

        . . .
    }
}

```
- To update the database to reflect the addition of the `Shipped` property to the `Order` class, open a new command prompt or PowerShell window, navigate to the SportsStore project folder and run the following command: 

```
dotnet ef migrations add ShippedOrders

```
The migration will be applied automatically when the application is started and the `SeedData` class calls the `Migrate` method provided by Entity Framework Core.

- Add to `AdminController` class `MarkShipped` method that will be receive a POST request that specifies the ID of an order, which is used to locate the corresponding `Order` object from the repository so that the `Shipped` property can be set to `true` and saved and  `Reset` method  that will be receive a POST request that specifies the ID of an order, which is used to locate the corresponding `Order` object from the repository so that the `Shipped` property can be set to `false` and saved

```
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class AdminController : Controller
    {
        . . .

        [HttpPost]
        public IActionResult MarkShipped(int orderId)
        {
            Order? order = orderRepository.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                order.Shipped = true;
                orderRepository.SaveOrder(order);
            }

            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult Reset(int orderId)
        {
            Order? order = orderRepository.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                order.Shipped = false;
                orderRepository.SaveOrder(order);
            }

            return RedirectToAction("Orders");
        }
    }
}
```
- To avoid duplicating code and content, create and add to the `Views/Order` folder a `_OrderTable.cshtml` Partial View that displays a table without knowing which category of order it is dealing with the content shown below

```
@model (IQueryable<Order> Orders, string TableTitle, string ButtonLabel, string CallbackMethodName)

<table class="table table-sm table-striped table-bordered">
    <thead>
        <tr><th colspan="5" class="text-center">@Model.TableTitle</th></tr>
    </thead>
    <tbody>
        @if (Model.Orders.Any())
        {
            @foreach (Order o in Model.Orders)
            {
                <tr>
                    <td>@o.Name</td>
                    <td>@o.Zip</td>
                    <th>Product</th>
                    <th>Quantity</th>
                    <td>
                        <form asp-action=@Model.CallbackMethodName method="post">
                            <input type="hidden" name="orderId" value="@o.OrderId" />
                            <button type="submit" class="btn btn-sm btn-danger">
                                @Model.ButtonLabel
                            </button>
                        </form>
                    </td>
                </tr>
                @foreach (CartLine line in o.Lines)
                {
                    <tr>
                        <td colspan="2"></td>
                        <td>@line.Product.Name</td>
                        <td>@line.Quantity</td>
                        <td></td>
                    </tr>
                }
            }
        }
        else
        {
            <tr><td colspan="5" class="text-center">No Orders</td></tr>
        }
    </tbody>
</table>
```
- Change a `Orders.cshtml` View that gets the `Order` data from the database and uses the `_OrderTable.cshtml` Partial View to display it to the user

```
@model IQueryable<Order>

@{
    Layout = "_AdminLayout";
    var unshippedOrders = Model.Where(o => !o.Shipped);
    var shippedOrders = Model.Where(o => o.Shipped);
}

<partial name="_OrderTable" model='(unshippedOrders, "Unshipped Orders", "Ship", "MarkShipped")' />
<partial name="_OrderTable" model='(shippedOrders, "Shipped Orders", "Reset", "Reset")' />
<form asp-action="Orders" method="post">
    <button class="btn btn-info">Refresh Data</button>
</form>
```
- To see your changes, build project, run application and request http://localhost:5000/Admin/Orders.

![](Images/4.3.png)

- To see the new features, request http://localhost:5000, and create an order. Once you have at least one order in the database, request http://localhost:5000/Admin/Orders, and you will see a summary of the order you created displayed in the `Unshipped Orders table`. Click the `Ship` button, and the order will be updated and moved to the `Shipped Orders table`, as shown below

![](Images/4.4.png)

![](Images/4.5.png)

Click the `Reset` button, and the order will be updated and moved to the `Unshipped Orders table`, as shown below

![](Images/4.6.png)

</details>

<details>
<summary>

**Adding Catalog Management**

</summary>


- To add the features that allow a administrator to create, modify, and delete products add new methods to the `IStoreRepository` interface

```
namespace SportsStore.Models.Repository
{
    public interface IStoreRepository
    {
        IQueryable<Product> Products { get; }

        void SaveProduct(Product p);

        void CreateProduct(Product p);

        void DeleteProduct(Product p);
    }
}

```

- Add implemention of this methods in the `EFStoreRepository` class (the `SportsStore/Models` folder)

```
namespace SportsStore.Models.Repository
{
    public class EFStoreRepository : IStoreRepository
    {
        private StoreDbContext context;

        public EFStoreRepository(StoreDbContext ctx)
        {
            this.context = ctx;
        }

        public IQueryable<Product> Products => this.context.Products;

        public void CreateProduct(Product p)
        {
            context.Add(p);
            context.SaveChanges();
        }

        public void DeleteProduct(Product p)
        {
            context.Remove(p);
            context.SaveChanges();
        }

        public void SaveProduct(Product p)
        {
            context.SaveChanges();
        }
    }
}

```

- To validate the values the user provides when editing or creating `Product` objects, add validation attributes to the `Product` data model class

```
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    public class Product
    {
        public long ProductId { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; } = string.Empty;
    }
}

```
- To provide the administrator a table of products with links to check and edit, replace the contents of the `Products.cshtml` file with those shown below

```
@model IQueryable<Product>

@{
    Layout = "_AdminLayout";
}

<table class="table table-sm table-striped table-bordered">
    <thead>
        <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Category</th>
            <th>Price</th>
            <td />
        </tr>
    </thead>
    <tbody>
        @if (Model?.Count() > 0)
        {
            @foreach (Product p in Model)
            {
                <tr>
                    <td>@p.ProductId</td>
                    <td>@p.Name</td>
                    <td>@p.Category</td>
                    <td>@p.Price.ToString("c")</td>
                    <td>
                        <a class="btn btn-info btn-sm" asp-controller="Admin" asp-action="Details">
                            Details
                        </a>
                        <a class="btn btn-warning btn-sm" asp-controller="Admin" asp-action="Edit">
                            Edit
                        </a>
                    </td>
                </tr>
            }
        }
        else
        {
            <tr>
                <td colspan="5" class="text-center">No Products</td>
            </tr>
        }
    </tbody>
</table>

<a class="btn btn-primary" asp-controller="Admin" asp-action="Create">Create</a>    
```

- Restart ASP.NET Core and request http://localhost:5000/Admin/Products

![](Images/4.7.png)

- To display all the fields for a single `Product` object add an `Details` action method in the `AdminController` class

```
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class AdminController : Controller
    {
        . . .
        [Route("Admin/Details/{productId:int}")]
        public ViewResult Details(int productId)
            => View(storeRepository.Products.FirstOrDefault(p => p.ProductId == productId));
        . . .
}
```
and a `Details.cshtml` view to the `Views/Admin` folder

```
@model SportsStore.Models.Product?

@{
    Layout = "_AdminLayout";
}

<h3 class="bg-info text-white text-center p-1">Details</h3>

<table class="table table-sm table-bordered table-striped">
    <tbody>
        <tr>
            <th>Id</th>
            <td>@Model?.ProductId</td>
        </tr>
        <tr>
            <th>Name</th>
            <td>@Model?.Name</td>
        </tr>
        <tr>
            <th>Description</th>
            <td>@Model?.Description</td>
        </tr>
        <tr>
            <th>Category</th>
            <td>@Model?.Category</td>
        </tr>
        <tr>
            <th>Price</th>
            <td>@Model?.Price.ToString("C")</td>
        </tr>
    </tbody>
</table>

<a class="btn btn-warning" asp-controller="Admin" asp-action="Edit" asp-route-productId="@Model?.ProductId">Edit</a>
<a class="btn btn-secondary" asp-controller="Admin" asp-action="Products">Back</a>
```
- Restart ASP.NET Core, request http://localhost:5000/Admin/Products and click `Details` link for some product

![](Images/4.8.png)

- To implement the abilities to edit and to create of a single `Product` object, add the `Edit` and `Create` action methods accordingly in the `AdminController` class.
```
public class AdminController : Controller
{
    . . .

    [Route("Admin/Edit/{productId:int}")]
    public ViewResult Edit(int productId)
        => View(storeRepository.Products.FirstOrDefault(p => p.ProductId == productId));
    
    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            storeRepository.SaveProduct(product);
            return RedirectToAction("Products");
        }

        return View(product);
    }

    . . .
}
```
//?????????????
- To support the operations to create and edit data, add a `Editor.cshtml` artial View to the `Pages/Admin` folder


- To see the editor work, restart ASP.NET Core, request http://localhost:5000/Admin/Products, and click the `Edit` button
  
![](Images/4.9.png)  

or request http://localhost:5000/admin, and click the `Create` button
  
    ![](Images/4.8.png)   

- To support the operations to delete, add in the `Products.razor` file in the `SportsStore/Pages/Admin` a `button`-tag and a `DeleteProduct` method

        @page "/admin/products"
        @page "/admin"

        @inherits OwningComponentBase<IStoreRepository>

        <table class="table table-sm table-striped table-bordered">
            <thead>
            <tr>
                <th>ID</th><th>Name</th>
                <th>Category</th><th>Price</th><td/>
            </tr>
            </thead>
            <tbody>
            @if (ProductData?.Count() > 0)
            {
                @foreach (Product p in ProductData)
                {
                    <tr>
                            ...
                            <button class="btn btn-danger btn-sm"
                                    @onclick="@(e => DeleteProduct(p))">
                                Delete
                            </button>

                        </td>
                    </tr>
                }
            }
            else
            {
                <tr>
                    <td colspan="5" class="text-center">No Products</td>
                </tr>
            }
            </tbody>
        </table>
        <NavLink class="btn btn-primary" href="/admin/products/create">Create</NavLink>

        @code {
            ...

            public async Task DeleteProduct(Product p)
            {
                Repository.DeleteProduct(p);
                await UpdateData();
            }

        }

-  Restart ASP.NET Core, request http://localhost:5000/admin/products, and click a `Delete` button to remove an object from the database

</details>

## Additional Materials

<details><summary>References
</summary> 

1. [Minimal APIs overview](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0)
1. [Get started with ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-6.0&tabs=visual-studio)
1. [Controllers](https://jakeydocs.readthedocs.io/en/latest/mvc/controllers/index.html)
1. [Views](https://jakeydocs.readthedocs.io/en/latest/mvc/views/index.html)
1. [Models](https://jakeydocs.readthedocs.io/en/latest/mvc/models/index.html)
1. [ASP.NET Core MVC with EF Core - tutorial series](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-6.0)
1. [Persist and retrieve relational data with Entity Framework Core](https://docs.microsoft.com/en-us/learn/modules/persist-data-ef-core/?view=aspnetcore-6.0)

</details>

<details><summary>Books
</summary> 

1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 1. Chapeter 9. SportsStore: Completing the Cart.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 2. Chapeter 13. Using URL Routing.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 2. Chapeter 14. Using Dependency Injection.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 2. Chapeter 15. Using the Platform Features. Part 1.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 2. Chapeter 16. Using the Platform Features. Part 2.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 2. Chapeter 17. Working with Data.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 3. Chapeter 21. Using Controllers with Views. Part 1.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 3. Chapeter 22. Using Controllers with Views. Part 2.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 3. Chapeter 24. Using View Components.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 3. Chapeter 28. Using Model Binding.
1. [Pro ASP.NET Core 6. Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition by Adam Freeman](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565/). Part 3. Chapeter 29. Using Model Validation.

</details>

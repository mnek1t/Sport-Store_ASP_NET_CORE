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

- Builed project, run application and request http://localhost:5000/. Your app should be work.

- To create a simple administration tool that will let to view the orders that have been received and mark them as shipped, at first change the data model so that adminstator can record which orders have been shipped. Add a `Shipped` property in the Order.cs file (the `Models` Folder)

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

dotnet ef database update
```
- Add action methods in the `OrderController.cs` file in the `SportsStore/Controllers` folder - the `List` method will be use to display a list of the unshipped orders to the administrator and the `MarkShipped` method will  be receive a POST request that specifies the ID of an order, which is used to locate the corresponding Order object from the repository so that the Shipped property can be set to true and saved.
 
```
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        . . .

        public ViewResult List() => View(orderRepository.Orders.Where(o => !o.Shipped));
        
        [HttpPost]
        public IActionResult MarkShipped(int orderId)
        {
            Order order = orderRepository
                .Orders
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                order.Shipped = true;
                orderRepository.SaveOrder(order);
            }

            return RedirectToAction(nameof(List));
        }

        . . .
    }
}

```
- To display the list of unshipped orders add a `List.cshtml` view file to the Views/Order folder and add the markup shown below

```
@model IQueryable<Order>

@{
    Layout = "_AdminLayout";
}

@if (Model.Any())
{
    <table class="table table-bordered table-striped">
        <tr>
            <th>Name</th>
            <th>Zip</th>
            <th colspan="2">Details</th>
            <th></th>
        </tr>
        @foreach (Order o in Model)
        {
            <tr>
                <td>@o.Name</td>
                <td>@o.Zip</td>
                <th>Product</th>
                <th>Quantity</th>
                <td>
                    <form asp-action="MarkShipped" method="post">
                        <input type="hidden" name="orderId" value="@o.OrderId" />
                        <button type="submit" class="btn btn-sm btn-danger">
                            Ship
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
    </table>
}
else
{
    <div class="text-center">No Unshipped Orders</div>
}

```
- Add a `_AdminLayout.cshtml` layout view in the Views/Shared folder with the following markup

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
                <div class="d-grid gap-1">
                    <a class="btn btn-outline-primary"
                       asp-action="List" asp-controller="Order">
                       Orders
                    </a>
                    <a class="btn btn-outline-primary"
                       asp-action="Products" asp-controller="Admin">
                        Products
                    </a>
                </div>
            </div>
            <div class="col-9">
                @RenderBody()
            </div>
        </div>
    </div>
</body>
</html>
```

- Build project, run application and request http://localhost:5000/Orders/List.

![](Images/4.1.png)

![](Images/4.2.png)

![](Images/4.3.png)

</details>

<details>
<summary>

**Adding Catalog Management**

</summary>


- To add the features that allow a administrator to create, read, update, and delete products add new methods to the `IStoreRepository` interface

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

- Add implemention of this methods in the `EFStoreRepository` calss (the SportsStore/Models folder)

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

- To validate the values the user provides when editing or creating Product objects, add validation attributes to the `Product` data model class

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


- To provide the administrator a table of products with links to check and edit, replace the contents of the `Products.razor` file

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
                        <td>@p.ProductId</td>
                        <td>@p.Name</td>
                        <td>@p.Category</td>
                        <td>@p.Price.ToString("c")</td>
                        <td>
                            <NavLink class="btn btn-info btn-sm"
                                     href="@GetDetailsUrl(p.ProductId)">
                                Details
                            </NavLink>
                            <NavLink class="btn btn-warning btn-sm"
                                     href="@GetEditUrl(p.ProductId)">
                                Edit
                            </NavLink>
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
            public IStoreRepository Repository => Service;

            public IEnumerable<Product> ProductData { get; set; }

            protected async override Task OnInitializedAsync()
            {
                await UpdateData();
            }

            public async Task UpdateData()
            {
                ProductData = await Repository.Products.ToListAsync();
            }

            public string GetDetailsUrl(long id) => $"/admin/products/details/{id}";

            public string GetEditUrl(long id) => $"/admin/products/edit/{id}";
        }

- Restart ASP.NET Core and request http://localhost:5000/admin/products

    ![](Images/4.3.png)

- To reate the Detail Component the job of that is to display all the fields for a single `Product` object, add a Razor Component named `Details.razor` to the `Pages/Admin` folder

        @page "/admin/products/details/{id:long}"
        
        <h3 class="bg-info text-white text-center p-1">Details</h3>
        <table class="table table-sm table-bordered table-striped">
            <tbody>
            <tr>
                <th>ID</th><td>@Product.ProductId</td>
            </tr>
            <tr>
                <th>Name</th><td>@Product.Name</td>
            </tr>
            <tr>
                <th>Description</th><td>@Product.Description</td>
            </tr>
            <tr>
                <th>Category</th><td>@Product.Category</td>
            </tr>
            <tr>
                <th>Price</th><td>@Product.Price.ToString("C")</td>
            </tr>
            </tbody>
        </table>
        <NavLink class="btn btn-warning" href="@EditUrl">Edit</NavLink>
        <NavLink class="btn btn-secondary" href="/admin/products">Back</NavLink>
        
        @code {
        
            [Inject]
            public IStoreRepository Repository { get; set; }
        
            [Parameter]
            public long Id { get; set; }
        
            public Product Product { get; set; }
        
            protected override void OnParametersSet()
            {
                Product = Repository.Products.FirstOrDefault(p => p.ProductId == Id);
            }
        
            public string EditUrl => $"/admin/products/edit/{Product.ProductId}";
        }

-  Restart ASP.NET Core, request http://localhost:5000/admin/products, and click one of the `Details` buttons
  
    ![](Images/4.6.png)

- To support the operations to create and edit data, add a Razor Component named `Editor.razor` to the `Pages/Admin` folder

        @page "/admin/products/edit/{id:long}"
        @page "/admin/products/create"

        @inherits OwningComponentBase<IStoreRepository>

        <style>
            div.validation-message { color: rgb(220, 53, 69); font-weight: 500 }
        </style>

        <h3 class="bg-@ThemeColor text-white text-center p-1">@TitleText a Product</h3>
        <EditForm Model="Product" OnValidSubmit="SaveProduct">
            <DataAnnotationsValidator/>
            @if (Product.ProductId != 0)
            {
                <div class="form-group">
                    <label>ID</label>
                    <input class="form-control" disabled value="@Product.ProductId"/>
                </div>
            }
            <div class="form-group">
                <label>Name</label>
                <ValidationMessage For="@(() => Product.Name)"/>
                <InputText class="form-control" @bind-Value="Product.Name"/>
            </div>
            <div class="form-group">
                <label>Description</label>
                <ValidationMessage For="@(() => Product.Description)"/>
                <InputText class="form-control" @bind-Value="Product.Description"/>
            </div>
            <div class="form-group">
                <label>Category</label>
                <ValidationMessage For="@(() => Product.Category)"/>
                <InputText class="form-control" @bind-Value="Product.Category"/>
            </div>
            <div class="form-group">
                <label>Price</label>
                <ValidationMessage For="@(() => Product.Price)"/>
                <InputNumber class="form-control" @bind-Value="Product.Price"/>
            </div>
            <button type="submit" class="btn btn-@ThemeColor">Save</button>
            <NavLink class="btn btn-secondary" href="/admin/products">Cancel</NavLink>
        </EditForm>

        @code {
            public IStoreRepository Repository => Service;

            [Inject]
            public NavigationManager NavManager { get; set; }

            [Parameter]
            public long Id { get; set; } = 0;

            public Product Product { get; set; } = new Product();

            protected override void OnParametersSet()
            {
                if (Id != 0)
                {
                    Product = Repository.Products.FirstOrDefault(p => p.ProductId == Id);
                }
            }

            public void SaveProduct()
            {
                if (Id == 0)
                {
                    Repository.CreateProduct(Product);
                }
                else
                {
                    Repository.SaveProduct(Product);
                }
                NavManager.NavigateTo("/admin/products");
            }

            public string ThemeColor => Id == 0 ? "primary" : "warning";

            public string TitleText => Id == 0 ? "Create" : "Edit";
        }

- To see the editor, restart ASP.NET Core, request http://localhost:5000/admin, and click the `Edit` button
  
    ![](Images/4.3.png)  

    ![](Images/4.7.png)   

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

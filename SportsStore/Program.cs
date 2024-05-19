using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using SportsStore.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StoreDbContext>(opts => {
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
});

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();

var app = builder.Build();

app.UseStaticFiles();

// Shows the specified page (in this case, page 2), showing items from all categories
app.MapControllerRoute(
    name: "pagination",
    pattern: "Products/Page{productPage:int}",
    defaults: new { Controller = "Home", action = "Index", productPage = 1 });

// Shows the specified page (in this case, page 1) of items from the specified category
app.MapControllerRoute(
     name: "categoryPage",
     pattern: "{category}/Page{productPage:int}",
     defaults: new { Controller = "Home", action = "Index" });

// Shows the first page of items from a specific category
app.MapControllerRoute(
    name: "category",
    pattern: "Products/{category}",
    defaults: new { Controller = "Home", action = "Index", productPage = 1 });

// Shows the first page of products from all categories
app.MapControllerRoute(
    name: "default",
    pattern: "/",
    defaults: new { Controller = "Home", action = "Index" });

app.MapDefaultControllerRoute();

// seed the database when the application starts,
SeedData.EnsurePopulated(app);

app.Run();

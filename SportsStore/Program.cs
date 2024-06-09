using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using SportsStore.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var sportsStoreConnectionString = builder.Configuration["ConnectionStrings:SportsStoreConnection"] ?? string.Empty;
var identityConnectionString = builder.Configuration["ConnectionStrings:IdentityConnection"] ?? string.Empty;

builder.Services.AddDbContext<StoreDbContext>(opts => 
{
    opts.UseSqlServer(sportsStoreConnectionString);
});

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(SessionCart.GetCart);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(identityConnectionString));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();

var app = builder.Build();

app.UseStatusCodePages();
app.UseStaticFiles();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
}

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

// Show roating for cart
app.MapControllerRoute(
      name: "shoppingCart",
      pattern: "Cart",
      defaults: new { Controller = "Cart", action = "Index" });

// Remove from cart route
app.MapControllerRoute(
      "remove",
      "Remove",
      new { Controller = "Cart", action = "Remove" });

// Show checkout route
app.MapControllerRoute(
      "checkout",
      "Checkout",
      new { Controller = "Order", action = "Checkout" });

// Shows the first page of products from all categories
app.MapControllerRoute(
    name: "default",
    pattern: "/",
    defaults: new { Controller = "Home", action = "Index" });

app.MapControllerRoute(
      "error",
      "Error",
      new { Controller = "Home", action = "Error" });

// seed the database when the application starts,
SeedData.EnsurePopulated(app);
await IdentitySeedData.EnsurePopulated(app);
app.Run();

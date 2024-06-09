using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [Route("Login")]
        [AllowAnonymous]
        public ViewResult Login(Uri returnUrl)
        {
            string applicationUrl = this.configuration["ApplicationSettings:DefaultReturnUrl"] ?? string.Empty;
            ValidateDefaultUri(applicationUrl);
            return this.View(new LoginViewModel
            {
                ReturnUrl = returnUrl ?? new Uri(applicationUrl),
            });
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            ValidateViewModel(loginViewModel);
            if (this.ModelState.IsValid)
            {
                IdentityUser user = await this.userManager.FindByNameAsync(loginViewModel.Name);

                if (user != null)
                {
                    await this.signInManager.SignOutAsync();

                    if ((await this.signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false)).Succeeded)
                    {
                        return this.RedirectToAction("Products", "Admin");
                    }
                }

                this.ModelState.AddModelError(string.Empty, "Invalid name or password.");
            }

            return this.View(loginViewModel);
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout(Uri returnUrl)
        {
            await this.signInManager.SignOutAsync();
            string applicationUrl = this.configuration["ApplicationSettings:DefaultReturnUrl"] ?? string.Empty;
            ValidateDefaultUri(applicationUrl);

            return this.Redirect((returnUrl ?? new Uri(applicationUrl)).ToString());
        }

        private static void ValidateViewModel(LoginViewModel loginViewModel) 
        {
            if (loginViewModel is null)
            {
                throw new ArgumentNullException(nameof(loginViewModel));
            }
        }

        private static void ValidateDefaultUri(string applicationUrl)
        {
            if (string.IsNullOrEmpty(applicationUrl))
            {
                throw new ArgumentNullException(nameof(applicationUrl));
            }
        }
    }
}

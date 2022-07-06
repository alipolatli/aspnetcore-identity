using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMVC.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;

        public BaseController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void AddModelError(IdentityResult identityResult)
        {
            foreach (var item in identityResult.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }

        public Task<User> GetCurrentUser()
        {
            return _userManager.FindByNameAsync(User.Identity.Name);

        }
    }
}

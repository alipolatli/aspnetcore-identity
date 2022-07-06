using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMVC.Areas.Administration.ViewComponents
{
    public class AdminSideBarViewComponent:ViewComponent
    {
        UserManager<User> _userManager;

        public AdminSideBarViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var user = _userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            return View(user);
        }

    }
}

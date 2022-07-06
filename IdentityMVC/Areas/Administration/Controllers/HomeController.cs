using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMVC.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(HttpContext.User.Claims.ToList());
        }
       
    }
}

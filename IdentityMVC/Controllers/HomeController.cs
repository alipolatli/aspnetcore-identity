using IdentityMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityMVC.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "HomeMember");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
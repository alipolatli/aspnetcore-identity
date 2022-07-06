using IdentityMVC.Areas.Administration.Models.Dtos;
using IdentityMVC.Areas.Administration.Models.ViewModels;
using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMVC.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly RoleManager<Role> _roleManager;

        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleAddDto roleAddDto)
        {
            if (ModelState.IsValid)
            {
                var role = new Role
                {
                    Name = roleAddDto.Name,
                };
                var identityResult = await _roleManager.CreateAsync(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddModelError(identityResult);
                }
            }
            return View(roleAddDto);
        }

        public async Task<IActionResult> EditRole(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            var roleUpdateDto = new RoleUpdateDto
            {
                Id = role.Id,
                Name = role.Name
            };

            if (role != null)
            {
                return View(roleUpdateDto);
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleUpdateDto roleUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(roleUpdateDto.Id.ToString());
                role.Name = roleUpdateDto.Name;

                var identityResult = await _roleManager.UpdateAsync(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddModelError(identityResult);
                }
            }
            return View(roleUpdateDto);


        }

        public async Task<IActionResult> RemoveRole(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                var identityResult = await _roleManager.DeleteAsync(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");

                }
                AddModelError(identityResult);
            }
            return View("Index");
        }

        public async Task<IActionResult> AssignRole(int id)
        {
            var currentUser = await _userManager.FindByIdAsync(id.ToString());
            ViewBag.Username = currentUser.UserName;
            var roles = _roleManager.Roles;
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            TempData["UserId"] = currentUser.Id;

            IList<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>();

            RoleAssignViewModel roleAssignViewModel = new RoleAssignViewModel();
            foreach (var role in roles)
            {
                roleAssignViewModel = new RoleAssignViewModel
                {
                    RoleName = role.Name,
                    Exist= currentUserRoles.Contains(role.Name)
                    // RoleId = role.Id,
                    //UserId = currentUser.Id;
                    //Exist= currentUserRoles.Contains(role.Name)
                };
                //if (currentUserRoles.Contains(role.Name))
                //{
                //    roleAssignViewModel.Exist = true;
                //}
                //else
                //{
                //    roleAssignViewModel.Exist = false;
                //}
                roleAssignViewModels.Add(roleAssignViewModel);
            }

            return View(roleAssignViewModels);

        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(List<RoleAssignViewModel> roleAssignViewModels)
        {
            if (ModelState.IsValid)
            {

                var currentUser = await _userManager.FindByIdAsync(TempData["UserId"].ToString());
                foreach (var item in roleAssignViewModels)
                {
                    if (item.Exist)
                    {
                        var identityResult = await _userManager.AddToRoleAsync(currentUser, item.RoleName);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(currentUser, item.RoleName);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(roleAssignViewModels);
        }







        [NonAction]
        private void AddModelError(IdentityResult identityResult)
        {
            identityResult.Errors.ToList().ForEach(e =>
            {
                ModelState.AddModelError("", e.Description);
            });

        }
    }
}

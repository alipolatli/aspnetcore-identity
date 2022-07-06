using IdentityMVC.Filters;
using IdentityMVC.Models.Dtos;
using IdentityMVC.Models.Entities;
using IdentityMVC.Models.Enums;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace IdentityMVC.Controllers
{
    [Authorize(Roles="Member,Admin,Manager,Editor")]
    public class HomeMemberController : BaseController
    {

        public HomeMemberController(UserManager<User> userManager, SignInManager<User> signInManager) : base(userManager, signInManager)
        {

        }

        public async Task<IActionResult> Index()
        {

            var logginUser = await GetCurrentUser();
            //UserDto userDto = new UserDto
            //{
            //    Email = logginUser.Email,
            //    PhoneNumber = logginUser.PhoneNumber,
            //    UserName = logginUser.UserName,
            //};
            var userDto = logginUser.Adapt<UserDto>();
            return View(userDto);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserPasswordChangeDto userPasswordChangeDto)
        {
            if (ModelState.IsValid)
            {
                var logginUser = await GetCurrentUser();
                bool exist = await _userManager.CheckPasswordAsync(logginUser, userPasswordChangeDto.OldPassword);
                if (!exist)
                {
                    ModelState.AddModelError("", "Wrong password.");
                    return View(userPasswordChangeDto);
                }
                else
                {
                    var identityResult = await _userManager.ChangePasswordAsync(logginUser, userPasswordChangeDto.OldPassword, userPasswordChangeDto.NewPassword);
                    if (identityResult.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(logginUser);
                        await _signInManager.SignOutAsync();
                        ViewBag.Success = "true";
                        return RedirectToAction("LogIn", "Auths");
                    }
                    else
                    {
                        AddModelError(identityResult);
                    }
                }
            }
            return View(userPasswordChangeDto);
        }

        public async Task<IActionResult> EditInformation()
        {
            var logginUser = await GetCurrentUser();

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            var userDto = logginUser.Adapt<UserUpdateDto>();

            return View(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditInformation(UserUpdateDto userUpdateDto, IFormFile userPictureFile)
        {
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            if (ModelState.IsValid)
            {
                var logginUser = await GetCurrentUser();

                if (userPictureFile != null && userPictureFile.Length > 0)
                {

                    string userPictureName = Guid.NewGuid().ToString() + Path.GetExtension(userPictureFile.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/userimages", userPictureName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        userPictureFile.CopyTo(stream);

                        logginUser.Thumbnail = "/images/userimages/" + userPictureName;
                    }

                }


                logginUser.Email = userUpdateDto.Email;
                logginUser.PhoneNumber = userUpdateDto.PhoneNumber;
                logginUser.UserName = userUpdateDto.UserName;
                logginUser.FirstName = userUpdateDto.FirstName;
                logginUser.BirthDate = userUpdateDto.BirthDate;
                logginUser.Gender = (int)userUpdateDto.Gender;

                var identityResult = await _userManager.UpdateAsync(logginUser);

                if (identityResult.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(logginUser);
                    ViewBag.Success = "true";
                    //await _signInManager.SignOutAsync();
                    //return RedirectToAction("LogIn", "Auths");

                }
                else
                {
                    AddModelError(identityResult);
                }
            }

            return View(userUpdateDto);
        }


        [Authorize(Policy = "EnoughAge")]
        public IActionResult ViolentVideo()
        {
            return View();
        }

        public async Task<IActionResult> ExchangeRedirect()
        {
            var currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (!HttpContext.User.HasClaim(claim => claim.Type == "expiredatexchange"))
            {
                Claim expireDateExchangeClaim = new Claim("expiredatexchange",DateTime.Now.AddDays(30).ToString(), ClaimValueTypes.String);

                await _userManager.AddClaimAsync(currentUser, expireDateExchangeClaim);
            }
            return RedirectToAction("Exchange");
        }

        //[TypeFilter(typeof(FreeExchangeViewFilter))]
        [Authorize(Policy = "FreeExchangePolicy")]
        public IActionResult Exchange()
        {
            return View();
        }




    }
}

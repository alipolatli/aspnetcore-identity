using IdentityMVC.Helpers.MailHelpers;
using IdentityMVC.Models.Dtos;
using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace IdentityMVC.Controllers
{
    public class AuthsController : BaseController
    {

        readonly PasswordResetService _mailService;
        readonly EmailConfirmationService _emailConfirmationService;

        public AuthsController(UserManager<User> userManager, SignInManager<User> signInManager, PasswordResetService mailService, EmailConfirmationService emailConfirmationService) : base(userManager, signInManager)
        {

            _mailService = mailService;
            _emailConfirmationService = emailConfirmationService;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Models.Enums.Gender)));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserDto userDto, IFormFile userPictureFile)
        {
            ModelState.Remove("Thumbnail");
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Models.Enums.Gender)));
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    BirthDate = userDto.BirthDate,
                    Gender = (int)userDto.Gender,
                    MemberDate = DateTime.UtcNow,
                    Thumbnail = userDto.Thumbnail,
                };

                if (userPictureFile != null && userPictureFile.Length > 0)
                {

                    string userPictureName = Guid.NewGuid().ToString() + Path.GetExtension(userPictureFile.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/userimages", userPictureName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        userPictureFile.CopyTo(stream);

                        user.Thumbnail = "/images/userimages/" + userPictureName;
                    }

                }

                IdentityResult identityResult = await _userManager.CreateAsync(user, userDto.Password);
                if (identityResult.Succeeded)
                {
                    var notConfirmUser = await _userManager.FindByEmailAsync(user.Email);

                    string confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(notConfirmUser);
                    string confirmEmailLink = Url.Action("ConfirmEmail", "Auths", new
                    {
                        UserId = notConfirmUser.Id,
                        Token = confirmEmailToken,
                    }, protocol: HttpContext.Request.Scheme);

                    _emailConfirmationService.EmailConfirmSendEmail(confirmEmailLink, notConfirmUser.Email);
                    // await _userManager.ConfirmEmailAsync(notConfirmUser, confirmEmailToken);

                    //await _userManager.AddToRoleAsync(confirmUser, "Member");
                    return RedirectToAction("LogIn");
                }
                else
                {
                    AddModelError(identityResult);
                    return View(userDto);
                }
            }
            return View(userDto);
        }

        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var identityResult = await _userManager.ConfirmEmailAsync(user, Token);
            if (identityResult.Succeeded)
            {
                ViewBag.State = "Email has been successfully verified.";
                await _userManager.AddToRoleAsync(user, "Member");
                return View();
            }
            else
            {
                AddModelError(identityResult);
                ViewBag.State = "Email could not be successfully verified.";
            }
            return View();

        }

        [HttpGet]
        public IActionResult LogIn(string ReturnUrl)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, userLoginDto.RememberMe, false);
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Please verify your email. ");
                        return View(userLoginDto);
                    }

                    if (signInResult.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "HomeMember");
                    }

                    else if (signInResult.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Locked account. Try again after.");
                        return View(userLoginDto);
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);
                        int fail = await _userManager.GetAccessFailedCountAsync(user);
                        if (fail >= 4)
                        {
                            ModelState.AddModelError("", $"You have logged in unsuccessfully {fail} times. Account will be locked for 10 minutes after 3 unsuccessful logins.You have {5 - fail} left.");

                        }
                        if (fail >= 5)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(5));
                            ModelState.AddModelError("", "Account locked for 10 minutes due to 5 failed logins.");
                        }
                    }
                }
                ModelState.AddModelError(String.Empty, "Incorrect information.");
                return View(userLoginDto);
            }
            return View(userLoginDto);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetPassword()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(userResetPasswordDto.Email);

            if (user != null)
            {
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Auths", new
                {
                    UserId = user.Id,
                    Token = passwordResetToken,
                }, HttpContext.Request.Scheme);

                _mailService.PasswordResetSendEmail(passwordResetLink, userResetPasswordDto.Email);
            }
            else
            {
                ModelState.AddModelError("", "No such mail was found.");
                return View(userResetPasswordDto);
            }
            ViewBag.Success = "true";
            return View();//redirect to action
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string UserId, string Token)
        {
            TempData["token"] = Token;
            TempData["userId"] = UserId;

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm(UserResetNewPasswordDto userResetNewPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(TempData["userId"]);

                var token = TempData["token"].ToString();

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, userResetNewPasswordDto.Password);
                    if (identityResult.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        ViewBag.SuccessPasswordReset = "Password reset successfully.";
                        return RedirectToAction("LogIn", "Auths");
                    }
                    AddModelError(identityResult);
                }
            }
            return View(userResetNewPasswordDto);

        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (ReturnUrl.Contains("ViolentVideo"))
            {
                ViewBag.Message = "We are sorry.This page is for users over the age of 18.";
            }
            else
            {
                ViewBag.Message = "We are sorry.";

            }
            return View();
        }

        public IActionResult LogInWithGoogle(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ResponseThirdParty", "Auths",new {ReturnUrl= ReturnUrl});

            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return new ChallengeResult("Google",authenticationProperties);
        }

        public async Task<IActionResult> ResponseThirdParty(string ReturnUrl="/")
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, true);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction(ReturnUrl);
                }
                else
                {
                    User user = new User
                    {
                        Email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                        FirstName= externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value,
                        LastName= externalLoginInfo.Principal.FindFirst(ClaimTypes.Surname).Value
                    };
                    if (externalLoginInfo.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(" ","-").ToLower();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value;
                    }
                    var identityResult = await _userManager.CreateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        var identityResultLogin= await _userManager.AddLoginAsync(user, externalLoginInfo);
                        if (identityResultLogin.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, true);
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            AddModelError(identityResultLogin);
                        }
                    }
                    else
                    {
                        AddModelError(identityResult);
                    }
                    return RedirectToAction("Error","Home");
                }
            }
        }


    }




}

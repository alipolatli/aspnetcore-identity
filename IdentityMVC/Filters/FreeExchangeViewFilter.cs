using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace IdentityMVC.Filters
{
    public class FreeExchangeViewFilter : IAsyncActionFilter
    {

        UserManager<User> _userManager;

        public FreeExchangeViewFilter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var currentUser = await _userManager.FindByNameAsync(context.HttpContext.User.Identity.Name);

            if (!context.HttpContext.User.HasClaim(claim => claim.Type == "expiredatexchange"))
            {
                Claim expireDateExchangeClaim = new Claim("expiredatexchange", DateTime.Now.AddDays(30).ToString(), ClaimValueTypes.String);

                await _userManager.AddClaimAsync(currentUser, expireDateExchangeClaim);
            }

            await next.Invoke();
            return;


        }
    }

}

using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityMVC.ClaimProviders
{
    public class ClaimProvider : IClaimsTransformation
    {
        UserManager<User> _userManager;

        public ClaimProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = principal.Identity as ClaimsIdentity;

                var user = await _userManager.FindByNameAsync(claimsIdentity.Name);

                if (user != null)
                {
                    var userAge = DateTime.Now.Year - user.BirthDate.Year;
                    if (userAge>18)
                    {
                        if (!principal.HasClaim(claim=>claim.Type== "enoughage"))
                        {
                            Claim ageClaim= new Claim("enoughage", userAge.ToString(),ClaimValueTypes.String);
                            claimsIdentity.AddClaim(ageClaim);
                        }
                    }

                    //if (!principal.HasClaim(claim => claim.Type == "birthdate"))
                    //{
                    //    Claim birthDateClaim = new Claim("birthdate", Convert.ToString(user.BirthDate), ClaimValueTypes.String, "Internal");
                    //    claimsIdentity.AddClaim(birthDateClaim);
                    //}

                }
            }
            return principal;

        }
    }
}

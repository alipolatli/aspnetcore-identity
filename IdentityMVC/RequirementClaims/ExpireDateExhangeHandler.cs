using Microsoft.AspNetCore.Authorization;

namespace IdentityMVC.RequirementClaims
{
    public class ExpireDateExhangeHandler : AuthorizationHandler<ExpireDateExchangeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequirement requirement)
        {
            var expireDateClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "expiredatexchange" && claim.Value != null);

            if (expireDateClaim != null)
            {
                if (DateTime.Now < Convert.ToDateTime(expireDateClaim.Value))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }


    }

}

using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityMVC.CustomMembershipValidations
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            string[] digits = new string[] { "0", "1", "2", "3,", "4", "5", "6", "7", "8", "9" };

            foreach (var item in digits)
            {
                if (user.UserName.StartsWith(item))
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "userNameStartWishDigit",
                        Description = "Username cannot start with a digit."
                    });
                }

            }

            if (errors.Count == 0)
                return Task.FromResult(IdentityResult.Success);
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}

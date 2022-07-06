using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityMVC.CustomMembershipValidations
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "passwordContainsUsername",
                    Description = "Password cannot contain username value."
                });
            }
            if (password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "passwordContainsEmail",
                    Description = "Password cannot contain email value."
                });
            }

            if (errors.Count == 0)
                return Task.FromResult(IdentityResult.Success);
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}

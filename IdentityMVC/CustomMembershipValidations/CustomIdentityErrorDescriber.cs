using Microsoft.AspNetCore.Identity;

namespace IdentityMVC.CustomMembershipValidations
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = "invalidUserName",
                Description = $"Bu {userName} geçersizdir."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "duplicateEmail",
                Description = $"Bu {email} zaten kayıtlı."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "passwordToShort",
                Description = $" Şifreniz en az {length} karakter olmalıdır."
            };
        }

    }
}

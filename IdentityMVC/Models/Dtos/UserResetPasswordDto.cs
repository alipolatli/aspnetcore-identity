using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Models.Dtos
{
    public class UserResetPasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

      

    }
}

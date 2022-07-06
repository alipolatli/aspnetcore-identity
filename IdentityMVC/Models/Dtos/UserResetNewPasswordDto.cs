using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Models.Dtos
{
    public class UserResetNewPasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

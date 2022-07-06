using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Models.Dtos
{
    public class UserPasswordChangeDto
    {

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Display(Name = "NewPassword(Again)")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage = "Passwords are not compatible.")]
        public string NewPasswordConfirm { get; set; }
    }
}

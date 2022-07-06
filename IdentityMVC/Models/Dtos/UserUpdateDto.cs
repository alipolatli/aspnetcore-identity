using IdentityMVC.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Models.Dtos
{
    public class UserUpdateDto
    {

        public int UserId { get; set; }

        [Required]
        [MinLength(2)]
        public string UserName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        //public string Picture { get; set; }

    }
}

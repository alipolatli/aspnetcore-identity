using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Areas.Administration.Models.Dtos
{
    public class RoleUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


    }
}

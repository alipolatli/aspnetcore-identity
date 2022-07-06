using System.ComponentModel.DataAnnotations;

namespace IdentityMVC.Areas.Administration.Models.Dtos
{
    public class RoleAddDto
    {
        [Required]
        public string Name { get; set; }
    }
}

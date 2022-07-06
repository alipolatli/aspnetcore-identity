using Microsoft.AspNetCore.Identity;

namespace IdentityMVC.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Thumbnail { get; set; }

        public int Gender { get; set; }

        public DateTime MemberDate { get; set; }

        public DateTime BirthDate { get; set; }
    }
}

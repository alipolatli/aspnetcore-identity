using IdentityMVC.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityMVC.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "user-id")]
    public class UserRoleName : TagHelper
    {
        public int UserId { get; set; }

        UserManager<User> _userManager;

        public UserRoleName(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var currentUser= await _userManager.FindByIdAsync(UserId.ToString());
            var currentUserRoles= await _userManager.GetRolesAsync(currentUser);

            foreach (var item in currentUserRoles)
            {
                output.Content.Append($"-{item}-");
            }

            //_userManager.GetRolesAsync()
            //return base.ProcessAsync(context, output);
        }
    }
}

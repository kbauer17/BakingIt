using Microsoft.AspNetCore.Identity;

namespace BakingIt.ViewModels
{
    public class UserManagementViewModel
    {
        public IdentityRole IdentityRole { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public UserManagementViewModel()
        {
            Roles = new List<IdentityRole>();
        }

        public UserManagementViewModel(IdentityRole identityRole)
        {
            this.IdentityRole = identityRole;
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace BakingIt.Models
{
    public class ApplicationUser : IdentityUser
    {
        // add any extra profile fields here and add them to the AspNetUsers table (migration will do it automatically, if not using migrations then do it via SQL commands)
        public string? DisplayName { get; set; }
    }
}
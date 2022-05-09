using Microsoft.AspNetCore.Identity;

namespace GeoComment.Models
{
    public class User : IdentityUser
    {
       
        public string First_name { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}

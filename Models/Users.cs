using Microsoft.AspNetCore.Identity;

namespace Project1.Models
{
    public class Users : IdentityUser
    {
        public string Name { get; set; }
        public string? Picture { get; set; } 
    }
}

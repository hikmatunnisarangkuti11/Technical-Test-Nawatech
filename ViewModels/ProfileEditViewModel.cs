using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ProfileEditViewModel
{
    [Required]
    public string Name { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; } 

    [Display(Name = "Profile Picture")]
    public string? Picture { get; set; }
}

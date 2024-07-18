
using System.ComponentModel.DataAnnotations;

public class UserForAuthenticationDto
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password name is required")]
    public string Password { get; set; }
}
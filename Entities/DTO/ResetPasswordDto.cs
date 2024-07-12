using System.ComponentModel.DataAnnotations;

namespace Entities.Dto;
public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email {get; set;}

    [Required]
    public string Token {get; set;}

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password {get; set;}
}
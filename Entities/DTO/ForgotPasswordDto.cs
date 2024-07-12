using System.ComponentModel.DataAnnotations;

namespace Entities.Dto;
public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email {get; set;}
}
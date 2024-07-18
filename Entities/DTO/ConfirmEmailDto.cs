

using System.ComponentModel.DataAnnotations;
namespace Entities.Dto;
public class ConfirmEmailDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Entities.Dto;

public abstract class EmployeeForManipulationDto
{
    [Required(ErrorMessage ="Name is a required field")]
    [MaxLength(30, ErrorMessage ="maximum lenght for employee's name is 30 characters only")]
    public string Name {get; set;}

    [Range(18, 64, ErrorMessage = " Age is required and it must be between the age of 18 to 64")]
    public int Age {get; set;}

    [Required(ErrorMessage ="Position is a required field")]
    [MaxLength(20, ErrorMessage ="maximum length for positon is 20 characters")]
    public string Position {get; set;}
}
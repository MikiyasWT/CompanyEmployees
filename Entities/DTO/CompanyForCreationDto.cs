using System.Dynamic;

namespace Entities.Dto;

public class CompanyForCreationDto : CompanyForManipulationDto
{
 public IEnumerable<EmployeeForCreationDto>? Employees {get; set;}
}



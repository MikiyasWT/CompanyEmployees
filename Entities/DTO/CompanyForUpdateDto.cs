using System.Dynamic;

namespace Entities.Dto;

public class CompanyForUpdateDto : CompanyForManipulationDto
{
    public IEnumerable<EmployeeForCreationDto>? Employees {get; set;}
}
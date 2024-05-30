using System.Diagnostics;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(Guid companyId,[FromQuery]EmployeeParameters employeeParameters)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"there is no such company with this ID {companyId}");
                return NotFound();
            }
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return Ok(employeesDto);

        }

        [HttpGet("{employeeId}", Name = "EmployeeById")]
        public async Task<IActionResult> GetEmployee(Guid companyId, Guid employeeId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"there is no such company with this ID {companyId}");
                return NotFound();
            }

            var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges: false);
            if (employeeFromDb == null)
            {
                _logger.LogInfo($"Employee with id: {employeeId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return Ok(employeeDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {
           
               var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
               if (company == null)
                {
                    _logger.LogInfo($"there is no such company with this ID {companyId}");
                    return NotFound();
                }
                var employeeEntity = _mapper.Map<Employee>(employee);
                _repository.Employee.CreateEmployee(companyId, employeeEntity);
                await _repository.SaveAsync();

               var employeeReturned = _mapper.Map<EmployeeDto>(employeeEntity);
                return CreatedAtRoute("EmployeeById", new { companyId, employeeId = employeeReturned.Id }, employeeReturned);
                        
            
        }

       [HttpDelete("{id}")]
       [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id)
        {
            
            var employeeForCompany = HttpContext.Items["employee"] as Employee;
            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();

            return NoContent();


        }


        [HttpPut("{employeeId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid employeeId, [FromBody]EmployeeForUpdateDto employee)
        { 

            var employeeToUpdate = HttpContext.Items["employee"] as Employee;
           
            _mapper.Map(employee, employeeToUpdate);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{employeeId}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> PartialEmployeeUpdate(Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
        {
          if(patchDocument == null)
          {
            _logger.LogError($"the patch document sent is empty");
            return BadRequest();
          }

          
          var employeeToUpdate  = HttpContext.Items["employee"] as Employee;

          var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeToUpdate);

          patchDocument.ApplyTo(employeeToPatch, ModelState);

          TryValidateModel(employeeToPatch);
          
          if(!ModelState.IsValid)
          {
                _logger.LogError($"invalid model state for patch document object");
                return UnprocessableEntity(ModelState);
          }

          _mapper.Map(employeeToPatch, employeeToUpdate);

          await _repository.SaveAsync();

          return NoContent();
        }
    }
}
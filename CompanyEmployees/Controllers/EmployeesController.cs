using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Models;
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
        public async Task<IActionResult> GetEmployees(Guid companyId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"there is no such company with this ID {companyId}");
                return NotFound();
            }
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);
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
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {
            if (companyId == null)
            {
                _logger.LogError($"company id is null");
                return BadRequest($"company id isnot provided");
            }
            else
            {
                var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
                if (company == null)
                {
                    _logger.LogInfo($"there is no such company with this ID {companyId}");
                    return NotFound();
                }
                else
                {
                    if (employee == null)
                        {
                            _logger.LogError($"Employee  is null");
                            return BadRequest($"Employee information isnot provided");
                        }
                    else if(!TryValidateModel(employee)){
                            _logger.LogError($"invalid model state for the EmployeeForCreationDto object");
                            return UnprocessableEntity(ModelState);
                    }    
                    else
                        {
                            var employeeEntity = _mapper.Map<Employee>(employee);
                            _repository.Employee.CreateEmployee(companyId, employeeEntity);
                            await _repository.SaveAsync();

                            var employeeReturned = _mapper.Map<EmployeeDto>(employeeEntity);
                            return CreatedAtRoute("EmployeeById", new { companyId, employeeId = employeeReturned.Id }, employeeReturned);
                        }
                }


            }
        }

       [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges:false);
            if(company == null)
            {
                _logger.LogError($"Company with company Id of {companyId}");
                return NotFound();
            }
            var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId,id,trackChanges:false);
            if(employeeForCompany == null){
                _logger.LogError($"employee with  Id of {id}");
                return NotFound();
            }
            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();

            return NoContent();


        }


        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid employeeId, [FromBody]EmployeeForUpdateDto employee)
        {
            if(employeeId == null)
            {
                _logger.LogError($"the employee Id was not provided");
                return BadRequest();
            }
            if(!TryValidateModel(employee))
            {
                _logger.LogError($"invalid model state for the EmployeeForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }  
            var company = await _repository.Company.GetCompanyAsync(companyId,trackChanges:true);
            if(company == null)
            {
                _logger.LogInfo($"there is no company with this ID:{companyId}");
                return NotFound();
            }
           
            var employeeToUpdate = await _repository.Employee.GetEmployeeAsync(companyId, employeeId,trackChanges:true);
            if(employeeToUpdate == null)
            {
                _logger.LogInfo($"there is no employee with this ID:{employeeId} that works for {company.Name}");
                return NotFound();
            }

            _mapper.Map(employee, employeeToUpdate);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartialEmployeeUpdate(Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
        {
          if(patchDocument == null)
          {
            _logger.LogError($"the patch document sent is empty");
            return BadRequest();
          }
          var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges:false);
          if(company == null)
          {
            _logger.LogInfo($"Company with ID:{companyId} was not found");
            return NotFound();
          }
          var employeeToUpdate  = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges:true);
          if(employeeToUpdate == null)
          {
                _logger.LogInfo($"there is no employee with this ID:{employeeId} that works for {company.Name}");
                return NotFound();
          }

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
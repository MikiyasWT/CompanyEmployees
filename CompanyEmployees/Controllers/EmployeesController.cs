using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Models;
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
        public IActionResult GetEmployees(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"there is no such company with this ID {companyId}");
                return NotFound();
            }
            var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return Ok(employeesDto);

        }

        [HttpGet("{employeeId}", Name = "EmployeeById")]
        public IActionResult GetEmployee(Guid companyId, Guid employeeId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"there is no such company with this ID {companyId}");
                return NotFound();
            }

            var employeeFromDb = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
            if (employeeFromDb == null)
            {
                _logger.LogInfo($"Employee with id: {employeeId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult CreateEmployee(Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {
            if (companyId == null)
            {
                _logger.LogError($"company id is null");
                return BadRequest($"company id isnot provided");
            }
            else
            {
                var company = _repository.Company.GetCompany(companyId, trackChanges: false);
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
                    else
                        {
                            var employeeEntity = _mapper.Map<Employee>(employee);
                            Console.Write(employeeEntity);
                            _repository.Employee.CreateEmployee(companyId, employeeEntity);
                            _repository.Save();

                            var employeeReturned = _mapper.Map<EmployeeDto>(employeeEntity);
                            return CreatedAtRoute("EmployeeById", new { companyId, employeeId = employeeReturned.Id }, employeeReturned);
                        }
                }


            }
        }
    }
}
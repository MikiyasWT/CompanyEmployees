using AutoMapper;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        private readonly IMapper _mapper;
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);

        }


        //api/companies/id
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id {id} doesn't exist in the database");
                return NotFound();
            }
            else
            {
                var CompanyDto = _mapper.Map<CompanyDto>(company);
                return Ok(CompanyDto);
            }

        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> getCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);

            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError(" some ids are not valid");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("company collection sent from client is null");
                return BadRequest("company collection is null");
            }



            var companyEntites = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntites)
            {
                _repository.Company.CreateCompany(company);
            }

            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntites);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError($"company detail sent from client is null");
                return BadRequest("Company Detail is Null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the CompanyForCreationDto object");
                return UnprocessableEntity(ModelState);
            }

            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);

        }


        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            if (companyId == null)
            {
                _logger.LogError($"company Id is empty");
                return BadRequest();
            }

            var companyToDelete = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (companyToDelete == null)
            {
                _logger.LogError($"company with Id:{companyId} is empty");
                return NotFound();
            }

            _repository.Company.DeleteCompany(companyToDelete);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(Guid companyId, [FromBody] CompanyForUpdateDto company)
        {
            if (companyId == null)
            {
                _logger.LogError($"company ID wasn't provided");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the CompanyForCreationDto object");
                return UnprocessableEntity(ModelState);
            }

            var companyToUpdate = await _repository.Company.GetCompanyAsync(companyId, trackChanges: true);
            if (companyToUpdate == null)
            {
                _logger.LogError($"there was no company with this ID:{companyId}");
                return NotFound();
            }
            Console.Write(companyToUpdate);
            _mapper.Map(company, companyToUpdate);
            await _repository.SaveAsync();

            return NoContent();

        }
    }
}



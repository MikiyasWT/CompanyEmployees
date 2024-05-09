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
        public IActionResult GetCompanies()
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);

        }


        //api/companies/id
        [HttpGet("{id}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
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
        public IActionResult getCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
           if(ids == null)
           {
            _logger.LogError("Parameter ids is null");
            return BadRequest("Parameter ids is null");
           }
           var companyEntities = _repository.Company.GetByIds(ids, trackChanges:false);
           
           if(ids.Count() != companyEntities.Count())
           {
            _logger.LogError(" some ids are not valid");
            return NotFound();
           }

           var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
           return Ok(companiesToReturn);
        }

       [HttpPost("collection")]
       public IActionResult CreateCompanyCollection([FromBody]IEnumerable<CompanyForCreationDto> companyCollection)
       {
        if(companyCollection == null)
        {
              _logger.LogError("company collection sent from client is null");
              return BadRequest("company collection is null");
        }

        var companyEntites = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntites)
        {
            _repository.Company.CreateCompany(company);
        }

        _repository.Save();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntites);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
        return CreatedAtRoute("CompanyCollection", new {ids}, companyCollectionToReturn);
       }

        [HttpPost]
        public IActionResult CreateCompany([FromBody]CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError($"company detail sent from client is null");
                return BadRequest("Company Detail is Null");
            }
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);

        }


        [HttpDelete("{companyId}")]
        public IActionResult DeleteCompany(Guid companyId)
        {
          if(companyId ==  null)
          {
            _logger.LogError($"company Id is empty");
            return BadRequest();
          }

          var companyToDelete = _repository.Company.GetCompany(companyId, trackChanges:false);
          if(companyToDelete == null){
            _logger.LogError($"company with Id:{companyId} is empty");
            return NotFound();
          }

          _repository.Company.DeleteCompany(companyToDelete);
          _repository.Save();

          return NoContent();
        }

        [HttpPut("{companyId}")]
        public IActionResult UpdateCompany(Guid companyId, [FromBody]CompanyForUpdateDto company)
        {
             if(companyId == null)
             {
                _logger.LogError($"company ID wasn't provided");
                return BadRequest();
             }
             var companyToUpdate = _repository.Company.GetCompany(companyId, trackChanges:true);
             if(companyToUpdate == null)
             {
                _logger.LogError($"there was no company with this ID:{companyId}");
                return NotFound();
             }
             Console.Write(companyToUpdate);
             _mapper.Map(company,companyToUpdate);
             _repository.Save();

             return NoContent();

        }
    }
}



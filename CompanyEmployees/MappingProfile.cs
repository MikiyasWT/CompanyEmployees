using AutoMapper;
using Entities.Dto;
using Entities.Models;

namespace CompanyEmployees
{
    public class MappingProfile : Profile{
        public MappingProfile()
        {
            // for response/retrieval
            CreateMap<Company, CompanyDto>()
                    .ForMember(c => c.FullAddress,
                       opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
        
            //for creation
            CreateMap<CompanyForCreationDto, Company>();
            
            // for response/retrieval
            CreateMap<Employee, EmployeeDto>();
            
            //for creation
            CreateMap<EmployeeForCreationDto, Employee>();


        }
    }
    
}
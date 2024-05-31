using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext)
            :base(repositoryContext)
        {
        }

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(c => c.CompanyId.Equals(companyId)
             && (c.Age >= employeeParameters.MinAge && c.Age <= employeeParameters.MaxAge), trackChanges)
                                     .OrderBy(e => e.Name)
                                     .ToListAsync();

            return PagedList<Employee>
                    .ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);                         
        }           


        public async Task<Employee>  GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges) =>
            await FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id.Equals(employeeId), trackChanges)
                  .SingleOrDefaultAsync();

        public void CreateEmployee(Guid companyId, Employee employee){
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

    }
}

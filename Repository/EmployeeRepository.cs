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



        public async Task<IEnumerable<Employee>>  GetEmployeesAsync(Guid companyId,EmployeeParameters employeeParameters, bool trackChanges) =>
            await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                   .OrderBy(e => e.Name)
                   .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
                   .Take(employeeParameters.PageSize) 
                   .ToListAsync();


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

using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext)
            :base(repositoryContext)
        {
        }



        public async Task<IEnumerable<Employee>>  GetEmployeesAsync(Guid companyId, bool trackChanges) =>
            await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                   .OrderBy(e => e.Name)
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

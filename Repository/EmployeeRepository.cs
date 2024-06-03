﻿using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

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
            var employees = await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                                     .FilterEmployees(employeeParameters.MinAge,employeeParameters.MaxAge)
                                     .Search(employeeParameters.SearchTerm)
                                     .Sort(employeeParameters.OrderBy)
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

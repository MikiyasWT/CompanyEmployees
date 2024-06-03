using Entities.Models;

namespace Repository.Extensions;

public static class RepositoryEmployeeExtension
{


    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees,uint MinAge, uint MaxAge){
          return employees.Where(e => e.Age >= MinAge && e.Age <= MaxAge);
    }

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm){
        if(string.IsNullOrWhiteSpace(searchTerm))
            return employees;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return employees.Where(e => e.Name.ToLower().Contains(searchTerm));    
    }
}
using System.Reflection;
using System.Text;
using Entities.Models;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;


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

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return employees.OrderBy(e => e.Name);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return employees.OrderBy(e => e.Name);

            return employees.OrderBy(orderQuery);
        }
}
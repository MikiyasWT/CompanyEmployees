


// using Contracts;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;

// public class ValidateCompanyExistsAttribute : IAsyncActionFilter
// {
//     private readonly ILoggerManager _logger;
//     private readonly IRepositoryManager _repository;

//     public ValidateCompanyExistsAttribute(ILoggerManager logger, IRepositoryManager repository)
//     {
//         logger = _logger;
//         repository = _repository;
//     }


//    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
//         var id = (Guid)context.ActionArguments["companyId"];
//         var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

//         if(company == null)
//         {

//             _logger.LogInfo($"Company with id: {id} doesn't exist in the database");
//             context.Result = new NotFoundResult();
//         }
//         else
//          {
//             //sending back the company if company with id found
//            context.HttpContext.Items.Add("company", company);
//            await next();
//         }
//     }
// }





using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CompanyEmployees.ActionFilters
{
    public class ValidateCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateCompanyExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (Guid)context.ActionArguments["companyId"];
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("company", company);
                await next();
            }
        }
    }
}

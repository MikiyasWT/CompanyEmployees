


using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateCompanyExistsAttribute : IAsyncActionFilter
{
    private readonly ILoggerManager _logger;
    private readonly IRepositoryManager _repository;

    public ValidateCompanyExistsAttribute(ILoggerManager logger, IRepositoryManager repository)
    {
        logger = _logger;
        repository = _repository;
    }


   public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
        var id = (Guid)context.ActionArguments["companyId"];
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

        if(company == null)
        {

            _logger.LogInfo($"Company with id: {id} doesn't exist in the database");
            context.Result = new NotFoundResult();
        }
        else
         {
            //sending back the company if company with id found
           context.HttpContext.Items.Add("company", company);
           await next();
        }
    }
}
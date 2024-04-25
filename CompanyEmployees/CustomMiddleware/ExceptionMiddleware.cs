using System.Net;
using System.Text.Json;
using Contracts;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Http;

namespace CompanyEmployees.CustomMiddleware;


public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILoggerManager _logger;

    public GlobalExceptionHandlingMiddleware(
        ILoggerManager logger) =>
        _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            var  problem = new ErrorDetails()
            {
                Success = false,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server has occurred."
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
        }
    }
}
